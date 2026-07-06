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

    public UpdateTaskItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
        {
            return Result.Failure(new Error("Task.NotFound", "Taak niet gevonden."));
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.AssigneeId = request.AssigneeId;
        task.ReporterId = request.ReporterId;
        task.DueDate = request.DueDate;
        task.EstimatedHours = request.EstimatedHours;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
