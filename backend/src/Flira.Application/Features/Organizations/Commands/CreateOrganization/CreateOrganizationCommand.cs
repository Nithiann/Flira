using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Organizations.Commands.CreateOrganization;

public record CreateOrganizationCommand(string Name, string Description, string CreatorUserId) : IRequest<Result<Guid>>;
