using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Tasks.Commands.DeleteTask;

public record DeleteTaskItemCommand(Guid TaskId) : IRequest<Result>;
