using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateTaskItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {
        var column = await _context.BoardColumns
            .Include(c => c.Board)
            .FirstOrDefaultAsync(c => c.Id == request.BoardColumnId, cancellationToken);
        if (column == null || column.Board == null)
        {
            return Result.Failure<Guid>(new Error("Column.NotFound", "Kolom niet gevonden."));
        }

        var projectId = column.Board.ProjectId;

        // Resolve the initial state for the project
        var initialState = await _context.ProjectTaskStates
            .FirstOrDefaultAsync(s => s.ProjectId == projectId && s.IsInitial, cancellationToken);
        var initialStatus = initialState?.Name ?? "Backlog";

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            BoardColumnId = request.BoardColumnId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = initialStatus,
            AssigneeId = request.AssigneeId,
            ReporterId = request.ReporterId,
            DueDate = request.DueDate,
            EstimatedHours = request.EstimatedHours,
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(task.Id);
    }
}
