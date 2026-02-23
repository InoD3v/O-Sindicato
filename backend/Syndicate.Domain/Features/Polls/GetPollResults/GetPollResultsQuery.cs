using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Polls;

namespace Syndicate.Domain.Features.Polls.GetPollResults;

public sealed record GetPollResultsQuery(
    Guid PollId,
    Guid UserId) : IRequest<Result<PollResponse>>;
