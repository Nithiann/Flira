using System;
using System.Threading.Tasks;
using Flira.Application.Features.Dashboard.Queries.GetDashboardStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] Guid projectId)
    {
        if (projectId == Guid.Empty)
        {
            return BadRequest(new { Message = "ProjectId is verplicht." });
        }

        var query = new GetDashboardStatsQuery(projectId);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(result.Value);
    }
}
