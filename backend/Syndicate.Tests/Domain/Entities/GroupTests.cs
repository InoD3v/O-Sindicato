using FluentAssertions;
using Syndicate.Domain.Entities;

namespace Syndicate.Tests.Domain.Entities;

public class GroupTests
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var ownerId = Guid.NewGuid();

        var group = Group.Create("Test Group", "A description", ownerId);

        group.Name.Should().Be("Test Group");
        group.Description.Should().Be("A description");
        group.OwnerId.Should().Be(ownerId);
        group.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldGenerate8CharInviteCode()
    {
        var group = Group.Create("Test", null, Guid.NewGuid());

        group.InviteCode.Should().NotBeNullOrWhiteSpace();
        group.InviteCode.Should().HaveLength(8);
        group.InviteCode.Should().MatchRegex("^[A-Z0-9]+$");
    }

    [Fact]
    public void Create_WithNullDescription_ShouldAllowNull()
    {
        var group = Group.Create("Test", null, Guid.NewGuid());

        group.Description.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldInitializeEmptyCollections()
    {
        var group = Group.Create("Test", null, Guid.NewGuid());

        group.Members.Should().NotBeNull().And.BeEmpty();
        group.Debts.Should().NotBeNull().And.BeEmpty();
        group.Polls.Should().NotBeNull().And.BeEmpty();
    }
}
