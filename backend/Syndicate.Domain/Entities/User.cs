using Syndicate.Domain.Common;

namespace Syndicate.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;

    // Navigation
    public ICollection<Member> Memberships { get; private set; } = new List<Member>();

    private User() { }

    public static User Create(string username, string email, string passwordHash)
    {
        return new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash
        };
    }
}
