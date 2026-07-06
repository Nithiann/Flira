using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var organizationExists = await _context.Organizations
            .AnyAsync(o => o.Id == request.OrganizationId, cancellationToken);
        if (!organizationExists)
        {
            return Result.Failure<Guid>(new Error("Organization.NotFound", "Organisatie niet gevonden."));
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            OrganizationId = request.OrganizationId,
            Name = request.Name,
            Description = request.Description,
            Color = request.Color,
            Icon = request.Icon,
            CreatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);

        // Automatically create a default Board
        var board = new Board
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            Name = "Hoofdbord",
            CreatedAt = DateTime.UtcNow
        };

        _context.Boards.Add(board);

        // Automatically populate with default Kanban Columns
        var defaultColumns = new[]
        {
            new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "Backlog", Position = 0, CreatedAt = DateTime.UtcNow },
            new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "Todo", Position = 1, CreatedAt = DateTime.UtcNow },
            new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "In Progress", Position = 2, CreatedAt = DateTime.UtcNow },
            new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "Review", Position = 3, CreatedAt = DateTime.UtcNow },
            new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "Done", Position = 4, CreatedAt = DateTime.UtcNow }
        };

        _context.BoardColumns.AddRange(defaultColumns);

        // Seed default Project Task States (Workflows)
        var defaultStates = new[]
        {
            new ProjectTaskState { Id = Guid.NewGuid(), ProjectId = project.Id, Name = "Backlog", AllowedTransitionsJson = "[\"Todo\"]", IsInitial = true },
            new ProjectTaskState { Id = Guid.NewGuid(), ProjectId = project.Id, Name = "Todo", AllowedTransitionsJson = "[\"In Progress\",\"Backlog\"]", IsInitial = false },
            new ProjectTaskState { Id = Guid.NewGuid(), ProjectId = project.Id, Name = "In Progress", AllowedTransitionsJson = "[\"Review\",\"Todo\"]", IsInitial = false },
            new ProjectTaskState { Id = Guid.NewGuid(), ProjectId = project.Id, Name = "Review", AllowedTransitionsJson = "[\"Done\",\"In Progress\"]", IsInitial = false },
            new ProjectTaskState { Id = Guid.NewGuid(), ProjectId = project.Id, Name = "Done", AllowedTransitionsJson = "[\"Review\"]", IsInitial = false }
        };

        _context.ProjectTaskStates.AddRange(defaultStates);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(project.Id);
    }
}
