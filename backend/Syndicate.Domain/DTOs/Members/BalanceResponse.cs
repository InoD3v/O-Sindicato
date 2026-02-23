namespace Syndicate.Domain.DTOs.Members;

public sealed record BalanceResponse(
    Guid MemberId,
    Guid GroupId,
    decimal TotalBalance,
    decimal BlockedBalance,
    decimal AvailableBalance);
