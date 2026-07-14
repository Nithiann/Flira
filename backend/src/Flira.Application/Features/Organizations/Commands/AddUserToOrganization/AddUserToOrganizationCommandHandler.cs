using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Organizations.Commands.AddUserToOrganization;

public class AddUserToOrganizationCommandHandler : IRequestHandler<AddUserToOrganizationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AddUserToOrganizationCommandHandler(IApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result> Handle(AddUserToOrganizationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure(new Error("User.NotFound", "Gebruiker met dit e-mailadres is niet gevonden."));
        }

        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            return Result.Failure(new Error("Organization.NotFound", "Organisatie niet gevonden."));
        }

        var existingMembership = await _context.OrganizationUsers
            .FirstOrDefaultAsync(ou => ou.OrganizationId == request.OrganizationId && ou.UserId == user.Id, cancellationToken);
        if (existingMembership != null)
        {
            return Result.Failure(new Error("Organization.UserAlreadyExists", "Gebruiker is al lid van deze organisatie."));
        }

        var organizationUser = new OrganizationUser
        {
            OrganizationId = request.OrganizationId,
            UserId = user.Id,
            Role = request.Role
        };

        _context.OrganizationUsers.Add(organizationUser);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
