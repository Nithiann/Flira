using System.Threading;
using System.Threading.Tasks;
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
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
        {
            return Result.Failure<TaskItemResponseDto>(new Error("Task.NotFound", "Taak niet gevonden."));
        }

        var response = new TaskItemResponseDto(
            task.Id,
            task.BoardColumnId,
            task.Title,
            task.Description,
            task.Priority,
            task.Status,
            task.AssigneeId,
            task.ReporterId,
            task.DueDate,
            task.EstimatedHours,
            task.CreatedAt
        );

        return Result.Success(response);
    }
}
