using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Organizations.Commands.UpdateUserOrganizationRole;

public class UpdateUserOrganizationRoleCommandHandler : IRequestHandler<UpdateUserOrganizationRoleCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserOrganizationRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateUserOrganizationRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.Role != "Admin" && request.Role != "Member")
        {
            return Result.Failure(new Error("Organization.InvalidRole", "Ongeldige rol opgegeven. Kies Admin of Member."));
        }

        var membership = await _context.OrganizationUsers
            .FirstOrDefaultAsync(ou => ou.OrganizationId == request.OrganizationId && ou.UserId == request.UserId, cancellationToken);

        if (membership == null)
        {
            return Result.Failure(new Error("Organization.UserNotFound", "Gebruiker is geen lid van deze organisatie."));
        }

        if (membership.Role == "Owner")
        {
            return Result.Failure(new Error("Organization.OwnerRoleCannotBeModified", "De eigenaarsrol kan niet worden aangepast."));
        }

        membership.Role = request.Role;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
