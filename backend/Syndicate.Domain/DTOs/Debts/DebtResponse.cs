using Syndicate.Domain.Entities;

namespace Syndicate.Domain.DTOs.Debts;

public sealed record DebtResponse(
    Guid Id,
    Guid GroupId,
    Guid CreditorMemberId,
    string CreditorUsername,
    Guid DebtorMemberId,
    string DebtorUsername,
    decimal Amount,
    string Description,
    DebtStatus Status,
    DateTime CreatedAt);
