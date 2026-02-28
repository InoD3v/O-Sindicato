using Syndicate.Domain.Common;

namespace Syndicate.Domain.Entities;

public class Member : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid GroupId { get; private set; }
    public MemberRole Role { get; private set; } = MemberRole.Member;
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; private set; } = default!;
    public Group Group { get; private set; } = default!;
    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    private Member() { }

    public static Member Create(Guid userId, Guid groupId, MemberRole role = MemberRole.Member)
    {
        return new Member
        {
            UserId = userId,
            GroupId = groupId,
            Role = role
        };
    }
}
