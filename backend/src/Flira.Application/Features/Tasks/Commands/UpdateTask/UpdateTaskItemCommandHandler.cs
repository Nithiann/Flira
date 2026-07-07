using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskItemCommandHandler : IRequestHandler<UpdateTaskItemCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public UpdateTaskItemCommandHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .Include(t => t.BoardColumn)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
        {
            return Result.Failure(new Error("Task.NotFound", "Taak niet gevonden."));
        }

        var oldAssigneeId = task.AssigneeId;

        task.Title = request.Title;
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.AssigneeId = request.AssigneeId;
        task.ReporterId = request.ReporterId;
        task.DueDate = request.DueDate;
        task.EstimatedHours = request.EstimatedHours;

        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(request.AssigneeId) && request.AssigneeId != oldAssigneeId)
        {
            var boardId = task.BoardColumn?.BoardId ?? Guid.Empty;
            await _mediator.Publish(new Common.Events.TaskAssignedEvent(
                task.Id,
                boardId,
                task.Title,
                request.AssigneeId,
                request.ReporterId ?? ""), cancellationToken);
        }

        return Result.Success();
    }
}
