using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Organizations.Commands.AddUserToOrganization;

public record AddUserToOrganizationCommand(Guid OrganizationId, string Email, string Role) : IRequest<Result>;
