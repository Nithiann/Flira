using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid ProjectId) : IRequest<Result>;
