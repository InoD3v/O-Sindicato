using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Groups;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Groups.GetGroup;

public sealed class GetGroupHandler : IRequestHandler<GetGroupQuery, Result<GroupResponse>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IMemberRepository _memberRepository;

    public GetGroupHandler(IGroupRepository groupRepository, IMemberRepository memberRepository)
    {
        _groupRepository = groupRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Result<GroupResponse>> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId);
        if (group is null)
            return Result.Failure<GroupResponse>("Group not found.");

        var members = await _memberRepository.GetByGroupIdAsync(group.Id);

        var memberResponses = members.Select(m => new MemberResponse(
            m.Id,
            m.UserId,
            m.User.Username,
            m.Role,
            m.JoinedAt)).ToList();

        return Result.Success(new GroupResponse(
            group.Id,
            group.Name,
            group.Description,
            group.InviteCode,
            group.OwnerId,
            group.CreatedAt,
            memberResponses));
    }
}
