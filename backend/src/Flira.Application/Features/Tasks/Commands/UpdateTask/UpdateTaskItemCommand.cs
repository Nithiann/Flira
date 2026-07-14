using System;
using Flira.Domain.Enums;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Tasks.Commands.UpdateTask;

public record UpdateTaskItemCommand(
    Guid TaskId,
    string Title,
    string Description,
    TaskPriority Priority,
    string? AssigneeId,
    string? ReporterId,
    DateTime? DueDate,
    decimal? EstimatedHours) : IRequest<Result>;
