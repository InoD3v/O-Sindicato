using Syndicate.Domain.Common;

namespace Syndicate.Domain.Entities;

public class Group : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string InviteCode { get; private set; } = default!;
    public Guid OwnerId { get; private set; }

    // Navigation
    public User Owner { get; private set; } = default!;
    public ICollection<Member> Members { get; private set; } = new List<Member>();
    public ICollection<Debt> Debts { get; private set; } = new List<Debt>();
    public ICollection<Poll> Polls { get; private set; } = new List<Poll>();

    private Group() { }

    public static Group Create(string name, string? description, Guid ownerId)
    {
        return new Group
        {
            Name = name,
            Description = description,
            OwnerId = ownerId,
            InviteCode = Guid.NewGuid().ToString("N")[..8].ToUpper()
        };
    }
}
