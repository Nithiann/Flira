using System;
using System.Threading.Tasks;
using Flira.Api.Security;
using Flira.Application.Features.Tasks.Commands.CreateTask;
using Flira.Application.Features.Tasks.Commands.DeleteTask;
using Flira.Application.Features.Tasks.Commands.UpdateTask;
using Flira.Application.Features.Tasks.Commands.UpdateTaskStatus;
using Flira.Application.Features.Tasks.Queries.GetTask;
using Flira.Domain.Constants;
using Flira.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(Permissions.TaskCreate)]
    public async Task<IActionResult> Create([FromBody] CreateTaskModel model)
    {
        var command = new CreateTaskItemCommand(
            model.BoardColumnId,
            model.Title,
            model.Description,
            model.Priority,
            model.AssigneeId,
            model.ReporterId,
            model.DueDate,
            model.EstimatedHours);

        var result = await _mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { TaskId = result.Value });
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.TaskRead)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetTaskItemQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.TaskUpdate)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskModel model)
    {
        var command = new UpdateTaskItemCommand(
            id,
            model.Title,
            model.Description,
            model.Priority,
            model.AssigneeId,
            model.ReporterId,
            model.DueDate,
            model.EstimatedHours);

        var result = await _mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Taak succesvol bijgewerkt." });
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.TaskDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteTaskItemCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Taak succesvol verwijderd (soft delete)." });
    }

    [HttpPut("{id}/status")]
    [HasPermission(Permissions.TaskUpdate)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusModel model)
    {
        var command = new UpdateTaskStatusCommand(id, model.NewStatus, model.NewBoardColumnId);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Taakstatus succesvol bijgewerkt." });
    }
}

public record CreateTaskModel(
    Guid BoardColumnId,
    string Title,
    string Description,
    TaskPriority Priority,
    string? AssigneeId,
    string? ReporterId,
    DateTime? DueDate,
    decimal? EstimatedHours);

public record UpdateTaskModel(
    string Title,
    string Description,
    TaskPriority Priority,
    string? AssigneeId,
    string? ReporterId,
    DateTime? DueDate,
    decimal? EstimatedHours);

public record UpdateTaskStatusModel(
    string NewStatus,
    Guid NewBoardColumnId);
