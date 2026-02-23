using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Polls;

namespace Syndicate.Domain.Features.Polls.CastVote;

/// <summary>
/// UC04 — Member votes in a poll; weight = total Pika balance (BR05).
/// </summary>
public sealed record CastVoteCommand(
    Guid PollId,
    Guid PollOptionId,
    Guid UserId) : IRequest<Result<PollResponse>>;
