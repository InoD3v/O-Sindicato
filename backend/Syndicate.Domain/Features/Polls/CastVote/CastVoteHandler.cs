using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Polls;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Polls.CastVote;

public sealed class CastVoteHandler : IRequestHandler<CastVoteCommand, Result<PollResponse>>
{
    private readonly IPollRepository _pollRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CastVoteHandler(
        IPollRepository pollRepository,
        IMemberRepository memberRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _pollRepository = pollRepository;
        _memberRepository = memberRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PollResponse>> Handle(CastVoteCommand request, CancellationToken cancellationToken)
    {
        var poll = await _pollRepository.GetByIdWithOptionsAndVotesAsync(request.PollId);
        if (poll is null)
            return Result.Failure<PollResponse>("Poll not found.");

        if (!poll.IsActive)
            return Result.Failure<PollResponse>("This poll is no longer active.");

        if (poll.ExpiresAt.HasValue && poll.ExpiresAt.Value < DateTime.UtcNow)
            return Result.Failure<PollResponse>("This poll has expired.");

        var option = poll.Options.FirstOrDefault(o => o.Id == request.PollOptionId);
        if (option is null)
            return Result.Failure<PollResponse>("Invalid poll option.");

        var member = await _memberRepository.GetByUserAndGroupAsync(request.UserId, poll.GroupId);
        if (member is null)
            return Result.Failure<PollResponse>("You are not a member of this group.");

        if (await _pollRepository.HasUserVotedAsync(poll.Id, member.Id))
            return Result.Failure<PollResponse>("You have already voted in this poll.");

        // BR05 — Weight = total Pika balance (including escrowed)
        var totalBalance = await _transactionRepository.GetBalanceAsync(member.Id);
        var vote = Vote.Create(request.PollOptionId, member.Id, totalBalance);

        await _pollRepository.AddVoteAsync(vote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Re-fetch to get updated aggregates
        poll = await _pollRepository.GetByIdWithOptionsAndVotesAsync(request.PollId);

        var optionResponses = poll!.Options.Select(o => new PollOptionResponse(
            o.Id,
            o.Text,
            o.Votes.Sum(v => v.Weight),
            o.Votes.Count)).ToList();

        return Result.Success(new PollResponse(
            poll.Id,
            poll.GroupId,
            poll.Creator.User.Username,
            poll.Title,
            poll.Description,
            poll.IsActive,
            poll.ExpiresAt,
            poll.CreatedAt,
            optionResponses));
    }
}
