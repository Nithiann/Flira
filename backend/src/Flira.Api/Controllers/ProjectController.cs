using System;
using System.Threading.Tasks;
using Flira.Api.Security;
using Flira.Application.Features.Projects.Commands.CreateProject;
using Flira.Application.Features.Projects.Commands.DeleteProject;
using Flira.Application.Features.Projects.Commands.UpdateProject;
using Flira.Application.Features.Projects.Queries.GetProject;
using Flira.Application.Features.Projects.Queries.GetProjects;
using Flira.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(Permissions.ProjectRead)]
    public async Task<IActionResult> List([FromQuery] Guid organizationId)
    {
        var query = new GetProjectsQuery(organizationId);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [HasPermission(Permissions.ProjectCreate)]
    public async Task<IActionResult> Create([FromBody] CreateProjectModel model)
    {
        // OrganizationId must be passed in the model
        var command = new CreateProjectCommand(model.OrganizationId, model.Name, model.Description, model.Color, model.Icon);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { ProjectId = result.Value });
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.ProjectRead)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetProjectQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.ProjectUpdate)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectModel model)
    {
        var command = new UpdateProjectCommand(id, model.Name, model.Description, model.Color, model.Icon);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Project succesvol bijgewerkt." });
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.ProjectDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteProjectCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Project succesvol verwijderd (soft delete)." });
    }
}

public record CreateProjectModel(Guid OrganizationId, string Name, string Description, string Color, string Icon);
public record UpdateProjectModel(string Name, string Description, string Color, string Icon);
