using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Groups;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Groups.JoinGroup;

public sealed class JoinGroupHandler : IRequestHandler<JoinGroupCommand, Result<GroupResponse>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public JoinGroupHandler(
        IGroupRepository groupRepository,
        IMemberRepository memberRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _memberRepository = memberRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GroupResponse>> Handle(JoinGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByInviteCodeAsync(request.InviteCode);
        if (group is null)
            return Result.Failure<GroupResponse>("Invalid invite code.");

        if (await _memberRepository.ExistsAsync(request.UserId, group.Id))
            return Result.Failure<GroupResponse>("You are already a member of this group.");

        var member = Member.Create(request.UserId, group.Id);
        await _memberRepository.AddAsync(member);

        // BR01 — Genesis: +100 Pikas
        var genesis = Transaction.CreateGenesis(member.Id);
        await _transactionRepository.AddAsync(genesis);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new GroupResponse(
            group.Id,
            group.Name,
            group.Description,
            group.InviteCode,
            group.OwnerId,
            group.CreatedAt,
            new List<MemberResponse>()));
    }
}
