namespace Syndicate.Domain.DTOs.Polls;

public sealed record PollResponse(
    Guid Id,
    Guid GroupId,
    string CreatorUsername,
    string Title,
    string? Description,
    bool IsActive,
    DateTime? ExpiresAt,
    DateTime CreatedAt,
    List<PollOptionResponse> Options);

public sealed record PollOptionResponse(
    Guid Id,
    string Text,
    decimal TotalWeight,
    int VoteCount);
