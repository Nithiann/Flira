using System;
using Flira.Domain.Enums;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskItemCommand(
    Guid BoardColumnId,
    string Title,
    string Description,
    TaskPriority Priority,
    string? AssigneeId,
    string? ReporterId,
    DateTime? DueDate,
    decimal? EstimatedHours) : IRequest<Result<Guid>>;
