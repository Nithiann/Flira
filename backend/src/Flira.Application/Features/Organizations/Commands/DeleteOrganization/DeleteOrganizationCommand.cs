using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Organizations.Commands.DeleteOrganization;

public record DeleteOrganizationCommand(Guid Id) : IRequest<Result>;
