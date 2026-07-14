using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Organizations.Commands.RemoveUserFromOrganization;

public class RemoveUserFromOrganizationCommandHandler : IRequestHandler<RemoveUserFromOrganizationCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public RemoveUserFromOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(RemoveUserFromOrganizationCommand request, CancellationToken cancellationToken)
    {
        var membership = await _context.OrganizationUsers
            .FirstOrDefaultAsync(ou => ou.OrganizationId == request.OrganizationId && ou.UserId == request.UserId, cancellationToken);

        if (membership == null)
        {
            return Result.Failure(new Error("Organization.UserNotFound", "Gebruiker is geen lid van deze organisatie."));
        }

        if (membership.Role == "Owner")
        {
            return Result.Failure(new Error("Organization.OwnerCannotBeRemoved", "De eigenaar van de organisatie kan niet worden verwijderd."));
        }

        _context.OrganizationUsers.Remove(membership);

        // Clean up: Remove user from all teams belonging to this organization
        var teamIds = await _context.Teams
            .Where(t => t.OrganizationId == request.OrganizationId)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        if (teamIds.Any())
        {
            var teamMemberships = await _context.TeamUsers
                .Where(tu => tu.UserId == request.UserId && teamIds.Contains(tu.TeamId))
                .ToListAsync(cancellationToken);

            _context.TeamUsers.RemoveRange(teamMemberships);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
