using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Organizations.Commands.UpdateUserOrganizationRole;

public record UpdateUserOrganizationRoleCommand(Guid OrganizationId, string UserId, string Role) : IRequest<Result>;
