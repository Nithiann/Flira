using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    Guid OrganizationId,
    string Name,
    string Description,
    string Color,
    string Icon) : IRequest<Result<Guid>>;
