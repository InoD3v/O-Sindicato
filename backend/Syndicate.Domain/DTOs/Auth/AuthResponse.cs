namespace Syndicate.Domain.DTOs.Auth;

public sealed record AuthResponse(
    Guid UserId,
    string Username,
    string Email,
    string Token);
