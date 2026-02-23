using Syndicate.Domain.Entities;

namespace Syndicate.Domain.DTOs.Groups;

public sealed record MemberResponse(
    Guid MemberId,
    Guid UserId,
    string Username,
    MemberRole Role,
    DateTime JoinedAt);
