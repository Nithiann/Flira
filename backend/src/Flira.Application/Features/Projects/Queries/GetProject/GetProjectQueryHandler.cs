using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Projects.Queries.GetProject;

public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, Result<ProjectResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProjectQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProjectResponseDto>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .Include(p => p.Boards)
                .ThenInclude(b => b.Columns)
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            return Result.Failure<ProjectResponseDto>(new Error("Project.NotFound", "Project niet gevonden."));
        }

        var response = new ProjectResponseDto(
            project.Id,
            project.OrganizationId,
            project.Name,
            project.Description,
            project.Color,
            project.Icon,
            project.CreatedAt,
            project.Boards.Select(b => new BoardDto(
                b.Id,
                b.Name,
                b.Columns.OrderBy(c => c.Position).Select(c => new ColumnDto(
                    c.Id,
                    c.Name,
                    c.Position
                )).ToList()
            )).ToList()
        );

        return Result.Success(response);
    }
}
