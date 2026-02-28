using FluentAssertions;
using Syndicate.Domain.Entities;

namespace Syndicate.Tests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var user = User.Create("johndoe", "john@example.com", "hashed_pw");

        user.Username.Should().Be("johndoe");
        user.Email.Should().Be("john@example.com");
        user.PasswordHash.Should().Be("hashed_pw");
        user.Id.Should().NotBeEmpty();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_ShouldInitializeMembershipsCollection()
    {
        var user = User.Create("johndoe", "john@example.com", "hashed_pw");

        user.Memberships.Should().NotBeNull().And.BeEmpty();
    }
}
