using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syndicate.Domain.Features.Debts.AcceptDebt;
using Syndicate.Domain.Features.Debts.CancelDebt;
using Syndicate.Domain.Features.Debts.CreateDebt;
using Syndicate.Domain.Features.Debts.GetGroupDebts;
using Syndicate.Domain.Features.Debts.ResolveDebt;

namespace Syndicate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DebtsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DebtsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Create a new debt between two members in a group (UC01).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDebtRequest request)
    {
        var command = new CreateDebtCommand(
            request.GroupId,
            GetUserId(),
            request.DebtorMemberId,
            request.Amount,
            request.Description);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Created(string.Empty, result.Value);
    }

    /// <summary>
    /// Accept a pending debt (UC02). The debtor confirms and pikas are blocked.
    /// </summary>
    [HttpPost("{id:guid}/accept")]
    public async Task<IActionResult> Accept(Guid id)
    {
        var command = new AcceptDebtCommand(id, GetUserId());
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Resolve an active debt (UC03). The creditor confirms the favor was fulfilled.
    /// </summary>
    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id)
    {
        var command = new ResolveDebtCommand(id, GetUserId());
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Cancel a pending debt. Only the creditor can cancel.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var command = new CancelDebtCommand(id, GetUserId());
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// List all debts for a group.
    /// </summary>
    [HttpGet("group/{groupId:guid}")]
    public async Task<IActionResult> GetByGroup(Guid groupId)
    {
        var query = new GetGroupDebtsQuery(groupId, GetUserId());
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}

/// <summary>Request body for creating a debt.</summary>
public record CreateDebtRequest(Guid GroupId, Guid DebtorMemberId, decimal Amount, string Description);
