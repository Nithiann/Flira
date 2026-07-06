using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteTaskItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
        {
            return Result.Failure(new Error("Task.NotFound", "Taak niet gevonden."));
        }

        // Soft delete
        task.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
