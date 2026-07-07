using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Flira.Application.Features.Attachments.Commands.DeleteAttachment;
using Flira.Application.Features.Attachments.Commands.UploadAttachment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class AttachmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttachmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("tasks/{taskId}/attachments")]
    public async Task<IActionResult> Upload(Guid taskId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { Message = "Er is geen bestand geüpload." });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        using (var stream = file.OpenReadStream())
        {
            var command = new UploadAttachmentCommand(
                taskId,
                file.FileName,
                stream,
                file.ContentType,
                file.Length,
                userId
            );

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
            }

            return Ok(result.Value);
        }
    }

    [HttpDelete("attachments/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var command = new DeleteAttachmentCommand(id, userId);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Bijlage succesvol verwijderd." });
    }
}
