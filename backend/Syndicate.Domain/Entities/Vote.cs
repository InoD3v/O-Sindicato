using Syndicate.Domain.Common;

namespace Syndicate.Domain.Entities;

/// <summary>
/// BR05 — Vote weight is captured at the moment the vote is cast,
/// equal to the voter's total Pika balance (including escrowed).
/// </summary>
public class Vote : BaseEntity
{
    public Guid PollOptionId { get; private set; }
    public Guid MemberId { get; private set; }
    public decimal Weight { get; private set; }

    // Navigation
    public PollOption PollOption { get; private set; } = default!;
    public Member Member { get; private set; } = default!;

    private Vote() { }

    public static Vote Create(Guid pollOptionId, Guid memberId, decimal weight)
    {
        return new Vote
        {
            PollOptionId = pollOptionId,
            MemberId = memberId,
            Weight = weight
        };
    }
}
