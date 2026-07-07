using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Flira.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
using Flira.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using Flira.Application.Features.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var query = new GetNotificationsQuery(userId);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var command = new MarkNotificationAsReadCommand(id, userId);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Notificatie als gelezen gemarkeerd." });
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var command = new MarkAllNotificationsAsReadCommand(userId);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Alle notificaties als gelezen gemarkeerd." });
    }
}
