using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syndicate.Domain.Features.Groups.CreateGroup;
using Syndicate.Domain.Features.Groups.GetGroup;
using Syndicate.Domain.Features.Groups.JoinGroup;

namespace Syndicate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GroupsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Create a new group. The authenticated user becomes the owner/admin.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
    {
        var command = new CreateGroupCommand(request.Name, request.Description, GetUserId());
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Join a group using an invite code.
    /// </summary>
    [HttpPost("join")]
    public async Task<IActionResult> Join([FromBody] JoinGroupRequest request)
    {
        var command = new JoinGroupCommand(request.InviteCode, GetUserId());
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Get group details by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetGroupQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }
}

/// <summary>Request body for creating a group.</summary>
public record CreateGroupRequest(string Name, string? Description);

/// <summary>Request body for joining a group.</summary>
public record JoinGroupRequest(string InviteCode);
