using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Polls;

namespace Syndicate.Domain.Features.Polls.CreatePoll;

public sealed record CreatePollCommand(
    Guid GroupId,
    Guid UserId,
    string Title,
    string? Description,
    DateTime? ExpiresAt,
    List<string> Options) : IRequest<Result<PollResponse>>;
