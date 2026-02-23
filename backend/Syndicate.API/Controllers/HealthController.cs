using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syndicate.Infrastructure.Data;

namespace Syndicate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly SyndicateDbContext _dbContext;

    public HealthController(SyndicateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var canConnect = await _dbContext.Database.CanConnectAsync();

        return Ok(new
        {
            Status = canConnect ? "healthy" : "unhealthy",
            Database = canConnect,
            Timestamp = DateTime.UtcNow
        });
    }
}
