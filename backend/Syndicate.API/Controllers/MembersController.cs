using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syndicate.Domain.Features.Members.GetBalance;

namespace Syndicate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMediator _mediator;

    public MembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Get the balance breakdown for the authenticated member in a group.
    /// </summary>
    [HttpGet("groups/{groupId:guid}/balance")]
    public async Task<IActionResult> GetBalance(Guid groupId)
    {
        var query = new GetBalanceQuery(groupId, GetUserId());
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}
