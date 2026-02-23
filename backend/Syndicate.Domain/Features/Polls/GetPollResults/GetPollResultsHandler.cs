using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Polls;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Polls.GetPollResults;

public sealed class GetPollResultsHandler : IRequestHandler<GetPollResultsQuery, Result<PollResponse>>
{
    private readonly IPollRepository _pollRepository;
    private readonly IMemberRepository _memberRepository;

    public GetPollResultsHandler(IPollRepository pollRepository, IMemberRepository memberRepository)
    {
        _pollRepository = pollRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Result<PollResponse>> Handle(GetPollResultsQuery request, CancellationToken cancellationToken)
    {
        var poll = await _pollRepository.GetByIdWithOptionsAndVotesAsync(request.PollId);
        if (poll is null)
            return Result.Failure<PollResponse>("Poll not found.");

        var member = await _memberRepository.GetByUserAndGroupAsync(request.UserId, poll.GroupId);
        if (member is null)
            return Result.Failure<PollResponse>("You are not a member of this group.");

        var optionResponses = poll.Options.Select(o => new PollOptionResponse(
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
