namespace Syndicate.Domain.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string username, string email);
}
