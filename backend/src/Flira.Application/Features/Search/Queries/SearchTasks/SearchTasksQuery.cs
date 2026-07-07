using System;
using System.Collections.Generic;
using Flira.Domain.Enums;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Search.Queries.SearchTasks;

public record SearchTasksQuery(
    string? SearchTerm,
    Guid? ProjectId,
    string? AssigneeId,
    string? Status,
    TaskPriority? Priority,
    string? Labels,
    int PageNumber = 1,
    int PageSize = 10,
    string SortBy = "Title",
    string SortOrder = "asc") : IRequest<Result<SearchTasksResponseDto>>;

public record SearchTasksResponseDto(
    List<SearchTaskItemDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

public record SearchTaskItemDto(
    Guid Id,
    Guid BoardColumnId,
    string Title,
    string Description,
    TaskPriority Priority,
    string Status,
    string? AssigneeId,
    string? ReporterId,
    string? Labels,
    DateTime? DueDate,
    decimal? EstimatedHours,
    DateTime CreatedAt);
