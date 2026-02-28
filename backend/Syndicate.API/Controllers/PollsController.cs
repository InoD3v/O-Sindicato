using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syndicate.Domain.Features.Polls.CastVote;
using Syndicate.Domain.Features.Polls.CreatePoll;
using Syndicate.Domain.Features.Polls.GetPollResults;

namespace Syndicate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PollsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PollsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Create a new poll in a group (UC04).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePollRequest request)
    {
        var command = new CreatePollCommand(
            request.GroupId,
            GetUserId(),
            request.Title,
            request.Description,
            request.ExpiresAt,
            request.Options);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Created(string.Empty, result.Value);
    }

    /// <summary>
    /// Cast a weighted vote on a poll option. Weight = member's total balance (BR05).
    /// </summary>
    [HttpPost("{pollId:guid}/vote")]
    public async Task<IActionResult> Vote(Guid pollId, [FromBody] CastVoteRequest request)
    {
        var command = new CastVoteCommand(pollId, request.PollOptionId, GetUserId());
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Get poll results with weighted vote totals.
    /// </summary>
    [HttpGet("{pollId:guid}/results")]
    public async Task<IActionResult> Results(Guid pollId)
    {
        var query = new GetPollResultsQuery(pollId, GetUserId());
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }
}

/// <summary>Request body for creating a poll.</summary>
public record CreatePollRequest(
    Guid GroupId,
    string Title,
    string? Description,
    List<string> Options,
    DateTime? ExpiresAt);

/// <summary>Request body for casting a vote.</summary>
public record CastVoteRequest(Guid PollOptionId);
