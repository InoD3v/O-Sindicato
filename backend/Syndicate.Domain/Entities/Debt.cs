using Syndicate.Domain.Common;

namespace Syndicate.Domain.Entities;

public class Debt : BaseEntity
{
    public Guid GroupId { get; private set; }
    public Guid CreditorMemberId { get; private set; }
    public Guid DebtorMemberId { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = default!;
    public DebtStatus Status { get; private set; } = DebtStatus.Pending;

    // Navigation
    public Group Group { get; private set; } = default!;
    public Member Creditor { get; private set; } = default!;
    public Member Debtor { get; private set; } = default!;

    private Debt() { }

    public static Debt Create(
        Guid groupId,
        Guid creditorMemberId,
        Guid debtorMemberId,
        decimal amount,
        string description)
    {
        return new Debt
        {
            GroupId = groupId,
            CreditorMemberId = creditorMemberId,
            DebtorMemberId = debtorMemberId,
            Amount = amount,
            Description = description,
            Status = DebtStatus.Pending
        };
    }

    /// <summary>
    /// UC02 — Debtor accepts the debt, triggering escrow lock.
    /// </summary>
    public Result Accept()
    {
        if (Status != DebtStatus.Pending)
            return Result.Failure("Debt can only be accepted when pending.");

        Status = DebtStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// UC03 — Creditor confirms the agreement was fulfilled.
    /// </summary>
    public Result Settle()
    {
        if (Status != DebtStatus.Active)
            return Result.Failure("Debt can only be settled when active.");

        Status = DebtStatus.Settled;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Cancel a pending debt (before acceptance).
    /// </summary>
    public Result Cancel()
    {
        if (Status != DebtStatus.Pending)
            return Result.Failure("Debt can only be cancelled when pending.");

        Status = DebtStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
