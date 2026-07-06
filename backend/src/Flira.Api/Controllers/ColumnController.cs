using System;
using System.Threading.Tasks;
using Flira.Api.Security;
using Flira.Application.Features.Columns.Commands.CreateColumn;
using Flira.Application.Features.Columns.Commands.DeleteColumn;
using Flira.Application.Features.Columns.Commands.MoveColumn;
using Flira.Application.Features.Columns.Commands.UpdateColumn;
using Flira.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api/columns")]
[Authorize]
public class ColumnController : ControllerBase
{
    private readonly IMediator _mediator;

    public ColumnController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(Permissions.ProjectUpdate)]
    public async Task<IActionResult> Create([FromBody] CreateColumnModel model)
    {
        var command = new CreateColumnCommand(model.BoardId, model.Name);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { ColumnId = result.Value });
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.ProjectUpdate)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateColumnModel model)
    {
        var command = new UpdateColumnCommand(id, model.Name);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Kolom succesvol hernoemd." });
    }

    [HttpPut("{id}/move")]
    [HasPermission(Permissions.ProjectUpdate)]
    public async Task<IActionResult> Move(Guid id, [FromBody] MoveColumnModel model)
    {
        var command = new MoveColumnCommand(id, model.NewPosition);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Kolom succesvol verplaatst." });
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.ProjectUpdate)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteColumnCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Kolom succesvol verwijderd." });
    }
}

public record CreateColumnModel(Guid BoardId, string Name);
public record UpdateColumnModel(string Name);
public record MoveColumnModel(int NewPosition);
