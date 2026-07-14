using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Workflows.Commands.UpdateStateTransitions;

public class UpdateStateTransitionsCommandHandler : IRequestHandler<UpdateStateTransitionsCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateStateTransitionsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateStateTransitionsCommand request, CancellationToken cancellationToken)
    {
        var taskState = await _context.ProjectTaskStates
            .FirstOrDefaultAsync(s => s.ProjectId == request.ProjectId && s.Name.ToLower() == request.StateName.ToLower(), cancellationToken);

        if (taskState == null)
        {
            return Result.Failure(new Error("Workflow.StateNotFound", "Status stappen niet gevonden in dit project."));
        }

        taskState.AllowedTransitionsJson = JsonSerializer.Serialize(request.AllowedTransitions);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
