using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Groups;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Groups.CreateGroup;

public sealed class CreateGroupHandler : IRequestHandler<CreateGroupCommand, Result<GroupResponse>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGroupHandler(
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

    public async Task<Result<GroupResponse>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = Group.Create(request.Name, request.Description, request.UserId);
        await _groupRepository.AddAsync(group);

        // Owner joins as Admin
        var member = Member.Create(request.UserId, group.Id, MemberRole.Admin);
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
