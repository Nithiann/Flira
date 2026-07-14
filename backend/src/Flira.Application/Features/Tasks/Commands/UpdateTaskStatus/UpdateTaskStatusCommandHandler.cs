using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Common.Events;
using Flira.Application.Interfaces;
using Flira.Domain.States;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public UpdateTaskStatusCommandHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .Include(t => t.BoardColumn)
                .ThenInclude(c => c!.Board)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null || task.BoardColumn == null || task.BoardColumn.Board == null)
        {
            return Result.Failure(new Error("Task.NotFound", "Taak niet gevonden."));
        }

        var projectId = task.BoardColumn.Board.ProjectId;
        var boardId = task.BoardColumn.BoardId;

        // Load project task states
        var projectStates = await _context.ProjectTaskStates
            .Where(s => s.ProjectId == projectId)
            .ToListAsync(cancellationToken);

        var currentDbState = projectStates.FirstOrDefault(s => s.Name.Equals(task.Status, StringComparison.OrdinalIgnoreCase));
        var targetDbState = projectStates.FirstOrDefault(s => s.Name.Equals(request.NewStatus, StringComparison.OrdinalIgnoreCase));

        if (targetDbState == null)
        {
            return Result.Failure(new Error("Workflow.StateNotFound", $"Status '{request.NewStatus}' is niet geconfigureerd voor dit project."));
        }

        var allowedTransitions = new List<string>();
        if (currentDbState != null)
        {
            try
            {
                allowedTransitions = JsonSerializer.Deserialize<List<string>>(currentDbState.AllowedTransitionsJson) ?? new List<string>();
            }
            catch
            {
                // Fallback to empty list
            }
        }

        var currentState = TaskItemStates.CreateCustom(task.Status, allowedTransitions);
        var targetState = TaskItemStates.CreateCustom(request.NewStatus, Enumerable.Empty<string>());

        if (!currentState.CanTransitionTo(targetState))
        {
            return Result.Failure(new Error("Workflow.InvalidTransition", $"Transitie van '{task.Status}' naar '{request.NewStatus}' is niet toegestaan binnen dit project."));
        }

        var targetColumnExists = await _context.BoardColumns
            .AnyAsync(c => c.Id == request.NewBoardColumnId && c.BoardId == task.BoardColumn.BoardId, cancellationToken);
        if (!targetColumnExists)
        {
            return Result.Failure(new Error("Column.NotFound", "De doelkolom bestaat niet of hoort bij een ander bord."));
        }

        var oldStatus = task.Status;
        task.Status = request.NewStatus;
        task.BoardColumnId = request.NewBoardColumnId;
        
        if (request.NewStatus.Equals("Done", StringComparison.OrdinalIgnoreCase))
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else
        {
            task.CompletedAt = null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new TaskStatusUpdatedEvent(
            task.Id,
            boardId,
            oldStatus,
            request.NewStatus,
            request.NewBoardColumnId,
            request.UserId), cancellationToken);

        return Result.Success();
    }
}
