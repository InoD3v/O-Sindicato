using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Debts;

namespace Syndicate.Domain.Features.Debts.ResolveDebt;

/// <summary>
/// UC03 — Creditor confirms the debtor fulfilled the agreement.
/// </summary>
public sealed record ResolveDebtCommand(
    Guid DebtId,
    Guid UserId) : IRequest<Result<DebtResponse>>;
