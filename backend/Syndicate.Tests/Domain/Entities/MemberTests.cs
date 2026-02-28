using FluentAssertions;
using Syndicate.Domain.Entities;

namespace Syndicate.Tests.Domain.Entities;

public class MemberTests
{
    [Fact]
    public void Create_ShouldSetDefaultRoleAsMember()
    {
        var member = Member.Create(Guid.NewGuid(), Guid.NewGuid());

        member.Role.Should().Be(MemberRole.Member);
    }

    [Fact]
    public void Create_WithAdminRole_ShouldSetAdmin()
    {
        var member = Member.Create(Guid.NewGuid(), Guid.NewGuid(), MemberRole.Admin);

        member.Role.Should().Be(MemberRole.Admin);
    }

    [Fact]
    public void Create_ShouldSetJoinedAt()
    {
        var member = Member.Create(Guid.NewGuid(), Guid.NewGuid());

        member.JoinedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
