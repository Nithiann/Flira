using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(
    Guid ProjectId,
    string Name,
    string Description,
    string Color,
    string Icon) : IRequest<Result>;
