using System;
using Flira.Domain.Enums;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Tasks.Queries.GetTask;

public record GetTaskItemQuery(Guid TaskId) : IRequest<Result<TaskItemResponseDto>>;

public record TaskItemResponseDto(
    Guid Id,
    Guid BoardColumnId,
    string Title,
    string Description,
    TaskPriority Priority,
    string Status,
    string? AssigneeId,
    string? ReporterId,
    DateTime? DueDate,
    decimal? EstimatedHours,
    DateTime CreatedAt);
