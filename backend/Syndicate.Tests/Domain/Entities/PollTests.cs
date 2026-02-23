using FluentAssertions;
using Syndicate.Domain.Entities;

namespace Syndicate.Tests.Domain.Entities;

public class PollTests
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var groupId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var poll = Poll.Create(groupId, creatorId, "Best pizza?", "Vote now", expiresAt);

        poll.GroupId.Should().Be(groupId);
        poll.CreatorMemberId.Should().Be(creatorId);
        poll.Title.Should().Be("Best pizza?");
        poll.Description.Should().Be("Vote now");
        poll.ExpiresAt.Should().Be(expiresAt);
        poll.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Close_ShouldDeactivatePoll()
    {
        var poll = Poll.Create(Guid.NewGuid(), Guid.NewGuid(), "Test", null, null);

        poll.Close();

        poll.IsActive.Should().BeFalse();
        poll.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldInitializeEmptyOptions()
    {
        var poll = Poll.Create(Guid.NewGuid(), Guid.NewGuid(), "Test", null, null);

        poll.Options.Should().NotBeNull().And.BeEmpty();
    }
}
