using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Workflows.Commands.CreateCustomState;

public class CreateCustomStateCommandHandler : IRequestHandler<CreateCustomStateCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public CreateCustomStateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(CreateCustomStateCommand request, CancellationToken cancellationToken)
    {
        var projectExists = await _context.Projects
            .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (!projectExists)
        {
            return Result.Failure(new Error("Project.NotFound", "Project niet gevonden."));
        }

        var stateExists = await _context.ProjectTaskStates
            .AnyAsync(s => s.ProjectId == request.ProjectId && s.Name.ToLower() == request.Name.ToLower(), cancellationToken);
        if (stateExists)
        {
            return Result.Failure(new Error("Workflow.StateAlreadyExists", "Er bestaat al een status met deze naam binnen dit project."));
        }

        var json = JsonSerializer.Serialize(request.AllowedTransitions);

        var taskState = new ProjectTaskState
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            Name = request.Name,
            AllowedTransitionsJson = json,
            IsInitial = false
        };

        _context.ProjectTaskStates.Add(taskState);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
