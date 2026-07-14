using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Organizations.Commands.DeleteOrganization;

public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (organization == null)
        {
            return Result.Failure(new Error("Organization.NotFound", "Organisatie niet gevonden."));
        }

        organization.IsDeleted = true;

        // Cascade soft delete to all projects under this organization
        var projects = await _context.Projects
            .Where(p => p.OrganizationId == organization.Id)
            .ToListAsync(cancellationToken);

        var projectIds = projects.Select(p => p.Id).ToList();

        foreach (var project in projects)
        {
            project.IsDeleted = true;
        }

        // Cascade soft delete to all tasks belonging to these projects
        if (projectIds.Any())
        {
            var tasks = await _context.TaskItems
                .Where(t => _context.Boards
                    .Where(b => projectIds.Contains(b.ProjectId))
                    .SelectMany(b => b.Columns)
                    .Select(c => c.Id)
                    .Contains(t.BoardColumnId))
                .ToListAsync(cancellationToken);

            foreach (var task in tasks)
            {
                task.IsDeleted = true;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
