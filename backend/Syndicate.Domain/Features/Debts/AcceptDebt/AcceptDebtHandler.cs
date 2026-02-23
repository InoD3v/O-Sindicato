using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Debts;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Debts.AcceptDebt;

public sealed class AcceptDebtHandler : IRequestHandler<AcceptDebtCommand, Result<DebtResponse>>
{
    private readonly IDebtRepository _debtRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptDebtHandler(
        IDebtRepository debtRepository,
        IMemberRepository memberRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _debtRepository = debtRepository;
        _memberRepository = memberRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DebtResponse>> Handle(AcceptDebtCommand request, CancellationToken cancellationToken)
    {
        var debt = await _debtRepository.GetByIdAsync(request.DebtId);
        if (debt is null)
            return Result.Failure<DebtResponse>("Debt not found.");

        // Only the debtor can accept
        var debtorMember = await _memberRepository.GetByIdAsync(debt.DebtorMemberId);
        if (debtorMember is null || debtorMember.UserId != request.UserId)
            return Result.Failure<DebtResponse>("Only the debtor can accept this debt.");

        // BR03 — Validate available balance
        var totalBalance = await _transactionRepository.GetBalanceAsync(debt.DebtorMemberId);
        var blockedBalance = await _debtRepository.GetBlockedBalanceAsync(debt.DebtorMemberId);
        var availableBalance = totalBalance - blockedBalance;

        if (availableBalance < debt.Amount)
            return Result.Failure<DebtResponse>("Insufficient Pikas to guarantee this debt.");

        // Transition to Active (escrow lock)
        var result = debt.Accept();
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
