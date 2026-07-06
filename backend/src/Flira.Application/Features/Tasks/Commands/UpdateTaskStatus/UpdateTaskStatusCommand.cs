using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Tasks.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(
    Guid TaskId,
    string NewStatus,
    Guid NewBoardColumnId) : IRequest<Result>;
