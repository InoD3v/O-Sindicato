using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Debts;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Debts.ResolveDebt;

public sealed class ResolveDebtHandler : IRequestHandler<ResolveDebtCommand, Result<DebtResponse>>
{
    private readonly IDebtRepository _debtRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResolveDebtHandler(
        IDebtRepository debtRepository,
        IMemberRepository memberRepository,
        IUnitOfWork unitOfWork)
    {
        _debtRepository = debtRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DebtResponse>> Handle(ResolveDebtCommand request, CancellationToken cancellationToken)
    {
        var debt = await _debtRepository.GetByIdAsync(request.DebtId);
        if (debt is null)
            return Result.Failure<DebtResponse>("Debt not found.");

        // ERR_NOT_CREDITOR — Only the creditor can settle
        var creditorMember = await _memberRepository.GetByIdAsync(debt.CreditorMemberId);
        if (creditorMember is null || creditorMember.UserId != request.UserId)
            return Result.Failure<DebtResponse>("Only the creditor can confirm payment.");

        var result = debt.Settle();
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
