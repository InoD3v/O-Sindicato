using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Debts;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Debts.CancelDebt;

public sealed class CancelDebtHandler : IRequestHandler<CancelDebtCommand, Result<DebtResponse>>
{
    private readonly IDebtRepository _debtRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelDebtHandler(
        IDebtRepository debtRepository,
        IMemberRepository memberRepository,
        IUnitOfWork unitOfWork)
    {
        _debtRepository = debtRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DebtResponse>> Handle(CancelDebtCommand request, CancellationToken cancellationToken)
    {
        var debt = await _debtRepository.GetByIdAsync(request.DebtId);
        if (debt is null)
            return Result.Failure<DebtResponse>("Debt not found.");

        // Only the creditor can cancel
        var creditorMember = await _memberRepository.GetByIdAsync(debt.CreditorMemberId);
        if (creditorMember is null || creditorMember.UserId != request.UserId)
            return Result.Failure<DebtResponse>("Only the creditor can cancel this debt.");

        var result = debt.Cancel();
        if (result.IsFailure)
            return Result.Failure<DebtResponse>(result.Error!);

        _debtRepository.Update(debt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DebtResponse(
            debt.Id,
            debt.GroupId,
            debt.CreditorMemberId,
            debt.Creditor.User.Username,
            debt.DebtorMemberId,
            debt.Debtor.User.Username,
            debt.Amount,
            debt.Description,
            debt.Status,
            debt.CreatedAt));
    }
}
