using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Debts;

namespace Syndicate.Domain.Features.Debts.AcceptDebt;

/// <summary>
/// UC02 — Debtor accepts a pending debt, triggering escrow lock.
/// </summary>
public sealed record AcceptDebtCommand(
    Guid DebtId,
    Guid UserId) : IRequest<Result<DebtResponse>>;
