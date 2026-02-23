using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Polls;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Polls.CreatePoll;

public sealed class CreatePollHandler : IRequestHandler<CreatePollCommand, Result<PollResponse>>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IPollRepository _pollRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePollHandler(
        IMemberRepository memberRepository,
        IPollRepository pollRepository,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _pollRepository = pollRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PollResponse>> Handle(CreatePollCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByUserAndGroupAsync(request.UserId, request.GroupId);
        if (member is null)
            return Result.Failure<PollResponse>("You are not a member of this group.");

        var poll = Poll.Create(
            request.GroupId,
            member.Id,
            request.Title,
            request.Description,
            request.ExpiresAt);

        await _pollRepository.AddAsync(poll);

        foreach (var optionText in request.Options)
        {
            var option = PollOption.Create(poll.Id, optionText);
            poll.Options.Add(option);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var optionResponses = poll.Options.Select(o => new PollOptionResponse(
            o.Id, o.Text, 0m, 0)).ToList();

        return Result.Success(new PollResponse(
            poll.Id,
            poll.GroupId,
            member.User.Username,
            poll.Title,
            poll.Description,
            poll.IsActive,
            poll.ExpiresAt,
            poll.CreatedAt,
            optionResponses));
    }
}
