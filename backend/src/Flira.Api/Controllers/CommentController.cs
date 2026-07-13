using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Flira.Application.Features.Comments.Commands.CreateComment;
using Flira.Application.Features.Comments.Queries.GetComments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("tasks/{taskId}/comments")]
    public async Task<IActionResult> Create([FromRoute] Guid taskId, [FromBody] CreateCommentModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var command = new CreateCommentCommand(taskId, userId, model.Content);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { CommentId = result.Value });
    }

    [HttpGet("tasks/{taskId}/comments")]
    public async Task<IActionResult> Get([FromRoute] Guid taskId)
    {
        var query = new GetCommentsQuery(taskId);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(result.Value);
    }
}

public record CreateCommentModel(string Content);
