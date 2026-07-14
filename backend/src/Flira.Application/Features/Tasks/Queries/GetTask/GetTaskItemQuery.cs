using System;
using Flira.Domain.Enums;
using Flira.Shared;
using MediatR;

using System.Collections.Generic;
using Flira.Application.Features.Attachments.Commands.UploadAttachment;

namespace Flira.Application.Features.Tasks.Queries.GetTask;

public record GetTaskItemQuery(Guid TaskId) : IRequest<Result<TaskItemResponseDto>>;

public record TaskItemResponseDto(
    Guid Id,
    Guid BoardColumnId,
    Guid ProjectId,
    Guid BoardId,
    string Title,
    string Description,
    TaskPriority Priority,
    string Status,
    string? AssigneeId,
    string? AssigneeName,
    string? ReporterId,
    string? ReporterName,
    DateTime? DueDate,
    decimal? EstimatedHours,
    DateTime CreatedAt,
    List<AttachmentDto> Attachments);

