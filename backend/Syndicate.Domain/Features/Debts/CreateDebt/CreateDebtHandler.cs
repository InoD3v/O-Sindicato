using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Debts;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Debts.CreateDebt;

public sealed class CreateDebtHandler : IRequestHandler<CreateDebtCommand, Result<DebtResponse>>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IDebtRepository _debtRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDebtHandler(
        IMemberRepository memberRepository,
        IDebtRepository debtRepository,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _debtRepository = debtRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DebtResponse>> Handle(CreateDebtCommand request, CancellationToken cancellationToken)
    {
        // Resolve creditor's membership in the group
        var creditor = await _memberRepository.GetByUserAndGroupAsync(request.CreditorUserId, request.GroupId);
        if (creditor is null)
            return Result.Failure<DebtResponse>("You are not a member of this group.");

        var debtor = await _memberRepository.GetByIdAsync(request.DebtorMemberId);
        if (debtor is null || debtor.GroupId != request.GroupId)
            return Result.Failure<DebtResponse>("Debtor is not a member of this group.");

        // ERR_SELF_DEBT
        if (creditor.Id == debtor.Id)
            return Result.Failure<DebtResponse>("You cannot create a debt against yourself.");

        var debt = Debt.Create(request.GroupId, creditor.Id, debtor.Id, request.Amount, request.Description);
        await _debtRepository.AddAsync(debt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DebtResponse(
            debt.Id,
            debt.GroupId,
            debt.CreditorMemberId,
            creditor.User.Username,
            debt.DebtorMemberId,
            debtor.User.Username,
            debt.Amount,
            debt.Description,
            debt.Status,
            debt.CreatedAt));
    }
}
