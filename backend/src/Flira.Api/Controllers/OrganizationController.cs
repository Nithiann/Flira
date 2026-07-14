using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Flira.Api.Security;
using Flira.Application.Features.Organizations.Commands.AddUserToOrganization;
using Flira.Application.Features.Organizations.Commands.CreateOrganization;
using Flira.Application.Features.Organizations.Commands.UpdateOrganization;
using Flira.Application.Features.Organizations.Commands.DeleteOrganization;
using Flira.Application.Features.Organizations.Commands.RemoveUserFromOrganization;
using Flira.Application.Features.Organizations.Commands.UpdateUserOrganizationRole;
using Flira.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Flira.Application.Features.Organizations.Queries.GetOrganizations;

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

    [HttpGet]
    public async Task<IActionResult> GetUserOrganizations()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var query = new GetOrganizationsQuery(userId);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(result.Value);
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

    [HttpPut("{id}")]
    [HasPermission(Permissions.OrganizationUpdate)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationModel model)
    {
        var command = new UpdateOrganizationCommand(id, model.Name, model.Description);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Organisatie succesvol bijgewerkt." });
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.OrganizationDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteOrganizationCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Organisatie succesvol verwijderd." });
    }

    [HttpPost("{id}/members")]
    [HasPermission(Permissions.OrganizationMembersManage)]
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

    [HttpDelete("{id}/members/{userId}")]
    [HasPermission(Permissions.OrganizationMembersManage)]
    public async Task<IActionResult> RemoveMember(Guid id, string userId)
    {
        var command = new RemoveUserFromOrganizationCommand(id, userId);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Lid succesvol verwijderd uit de organisatie." });
    }

    [HttpPut("{id}/members/{userId}/role")]
    [HasPermission(Permissions.OrganizationMemberRolesManage)]
    public async Task<IActionResult> UpdateMemberRole(Guid id, string userId, [FromBody] UpdateMemberRoleModel model)
    {
        var command = new UpdateUserOrganizationRoleCommand(id, userId, model.Role);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(new { Message = "Rol van lid succesvol bijgewerkt." });
    }

    [HttpGet("{id}/members")]
    [HasPermission(Permissions.OrganizationMembersRead)]
    public async Task<IActionResult> GetMembers(Guid id)
    {
        var query = new Flira.Application.Features.Organizations.Queries.GetOrganizationMembers.GetOrganizationMembersQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(result.Value);
    }
}

public record CreateOrganizationModel(string Name, string Description);
public record UpdateOrganizationModel(string Name, string Description);
public record AddMemberModel(string Email, string Role);
public record UpdateMemberRoleModel(string Role);
