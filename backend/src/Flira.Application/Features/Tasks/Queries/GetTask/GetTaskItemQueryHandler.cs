using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Features.Attachments.Commands.UploadAttachment;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Tasks.Queries.GetTask;

public class GetTaskItemQueryHandler : IRequestHandler<GetTaskItemQuery, Result<TaskItemResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTaskItemQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TaskItemResponseDto>> Handle(GetTaskItemQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .Include(t => t.BoardColumn)
                .ThenInclude(c => c!.Board)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
        {
            return Result.Failure<TaskItemResponseDto>(new Error("Task.NotFound", "Taak niet gevonden."));
        }

        var projectId = task.BoardColumn?.Board?.ProjectId ?? Guid.Empty;
        var boardId = task.BoardColumn?.BoardId ?? Guid.Empty;
        var attachments = task.Attachments.Select(a => new AttachmentDto(
            a.Id,
            a.TaskItemId,
            a.FileName,
            a.FileUrl,
            a.FileSizeInBytes,
            a.UploadedById,
            a.CreatedAt
        )).ToList();

        // Resolve assignee and reporter names
        string? assigneeName = null;
        string? reporterName = null;
        
        if (!string.IsNullOrEmpty(task.AssigneeId))
        {
            var assignee = await _context.Users.FirstOrDefaultAsync(u => u.Id == task.AssigneeId, cancellationToken);
            assigneeName = assignee?.UserName;
        }
        if (!string.IsNullOrEmpty(task.ReporterId))
        {
            var reporter = await _context.Users.FirstOrDefaultAsync(u => u.Id == task.ReporterId, cancellationToken);
            reporterName = reporter?.UserName;
        }

        var response = new TaskItemResponseDto(
            task.Id,
            task.BoardColumnId,
            projectId,
            boardId,
            task.Title,
            task.Description,
            task.Priority,
            task.Status,
            task.AssigneeId,
            assigneeName,
            task.ReporterId,
            reporterName,
            task.DueDate,
            task.EstimatedHours,
            task.CreatedAt,
            attachments
        );

        return Result.Success(response);
    }
}

