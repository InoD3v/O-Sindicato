namespace Syndicate.Domain.DTOs.Groups;

public sealed record GroupResponse(
    Guid Id,
    string Name,
    string? Description,
    string InviteCode,
    Guid OwnerId,
    DateTime CreatedAt,
    List<MemberResponse> Members);
