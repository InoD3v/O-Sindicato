using Syndicate.Domain.Common;

namespace Syndicate.Domain.Entities;

public class PollOption : BaseEntity
{
    public Guid PollId { get; private set; }
    public string Text { get; private set; } = default!;

    // Navigation
    public Poll Poll { get; private set; } = default!;
    public ICollection<Vote> Votes { get; private set; } = new List<Vote>();

    private PollOption() { }

    public static PollOption Create(Guid pollId, string text)
    {
        return new PollOption
        {
            PollId = pollId,
            Text = text
        };
    }
}
