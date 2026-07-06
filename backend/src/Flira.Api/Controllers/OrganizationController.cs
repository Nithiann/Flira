using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Flira.Api.Security;
using Flira.Application.Features.Organizations.Commands.AddUserToOrganization;
using Flira.Application.Features.Organizations.Commands.CreateOrganization;
using Flira.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api/organizations")]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var command = new CreateOrganizationCommand(model.Name, model.Description, userId);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { OrganizationId = result.Value });
    }

    [HttpPost("{id}/members")]
    [HasPermission(Permissions.OrganizationManage)]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddMemberModel model)
    {
        var command = new AddUserToOrganizationCommand(id, model.Email, model.Role);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Lid succesvol toegevoegd aan de organisatie." });
    }
}

public record CreateOrganizationModel(string Name, string Description);
public record AddMemberModel(string Email, string Role);
