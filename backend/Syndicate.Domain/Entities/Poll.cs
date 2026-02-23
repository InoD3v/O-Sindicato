using Syndicate.Domain.Common;

namespace Syndicate.Domain.Entities;

public class Poll : BaseEntity
{
    public Guid GroupId { get; private set; }
    public Guid CreatorMemberId { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? ExpiresAt { get; private set; }

    // Navigation
    public Group Group { get; private set; } = default!;
    public Member Creator { get; private set; } = default!;
    public ICollection<PollOption> Options { get; private set; } = new List<PollOption>();

    private Poll() { }

    public static Poll Create(
        Guid groupId,
        Guid creatorMemberId,
        string title,
        string? description,
        DateTime? expiresAt)
    {
        return new Poll
        {
            GroupId = groupId,
            CreatorMemberId = creatorMemberId,
            Title = title,
            Description = description,
            ExpiresAt = expiresAt
        };
    }

    public void Close()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
