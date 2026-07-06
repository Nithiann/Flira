using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flira.Api.Security;
using Flira.Application.Features.Workflows.Commands.CreateCustomState;
using Flira.Application.Features.Workflows.Commands.UpdateStateTransitions;
using Flira.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId}/workflow")]
[Authorize]
public class WorkflowController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkflowController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("states")]
    [HasPermission(Permissions.ProjectUpdate)]
    public async Task<IActionResult> CreateState(Guid projectId, [FromBody] CreateStateModel model)
    {
        var command = new CreateCustomStateCommand(projectId, model.Name, model.AllowedTransitions);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Custom status succesvol aangemaakt." });
    }

    [HttpPut("states/{stateName}/transitions")]
    [HasPermission(Permissions.ProjectUpdate)]
    public async Task<IActionResult> UpdateTransitions(Guid projectId, string stateName, [FromBody] UpdateTransitionsModel model)
    {
        var command = new UpdateStateTransitionsCommand(projectId, stateName, model.AllowedTransitions);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Statusovergangen succesvol bijgewerkt." });
    }
}

public record CreateStateModel(string Name, List<string> AllowedTransitions);
public record UpdateTransitionsModel(List<string> AllowedTransitions);
