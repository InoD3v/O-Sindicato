using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Debts;

namespace Syndicate.Domain.Features.Debts.CreateDebt;

/// <summary>
/// UC01 — Creditor registers a debt against another member.
/// </summary>
public sealed record CreateDebtCommand(
    Guid GroupId,
    Guid CreditorUserId,
    Guid DebtorMemberId,
    decimal Amount,
    string Description) : IRequest<Result<DebtResponse>>;
