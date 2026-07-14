using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Organizations.Commands.UpdateOrganization;

public record UpdateOrganizationCommand(Guid Id, string Name, string Description) : IRequest<Result>;
