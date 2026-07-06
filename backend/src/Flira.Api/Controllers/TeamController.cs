using System;
using System.Threading.Tasks;
using Flira.Api.Security;
using Flira.Application.Features.Teams.Commands.AddUserToTeam;
using Flira.Application.Features.Teams.Commands.CreateTeam;
using Flira.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize]
public class TeamController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(Permissions.TeamManage)]
    public async Task<IActionResult> Create([FromBody] CreateTeamModel model)
    {
        var command = new CreateTeamCommand(model.OrganizationId, model.Name, model.Description);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { TeamId = result.Value });
    }

    [HttpPost("{id}/members")]
    [HasPermission(Permissions.TeamManage)]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddTeamMemberModel model)
    {
        var command = new AddUserToTeamCommand(id, model.Email);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Lid succesvol toegevoegd aan het team." });
    }
}

public record CreateTeamModel(Guid OrganizationId, string Name, string Description);
public record AddTeamMemberModel(string Email);
