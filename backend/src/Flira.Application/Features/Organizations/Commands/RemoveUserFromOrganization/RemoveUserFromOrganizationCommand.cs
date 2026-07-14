using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Organizations.Commands.RemoveUserFromOrganization;

public record RemoveUserFromOrganizationCommand(Guid OrganizationId, string UserId) : IRequest<Result>;
