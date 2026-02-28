using Syndicate.Domain.Common;

namespace Syndicate.Domain.Entities;

/// <summary>
/// Ledger entry — immutable record of every credit/debit in a member's wallet.
/// Balance = SUM(Amount) for a given MemberId.
/// </summary>
public class Transaction : BaseEntity
{
    public Guid MemberId { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public string Description { get; private set; } = default!;
    public Guid? DebtId { get; private set; }

    // Navigation
    public Member Member { get; private set; } = default!;
    public Debt? Debt { get; private set; }

    private Transaction() { }

    /// <summary>
    /// BR01 — Creates the genesis entry (+100 Pikas) when a user joins a group.
    /// </summary>
    public static Transaction CreateGenesis(Guid memberId)
    {
        return new Transaction
        {
            MemberId = memberId,
            Amount = 100m,
            Type = TransactionType.Genesis,
            Description = "Initial balance upon joining group"
        };
    }
}
