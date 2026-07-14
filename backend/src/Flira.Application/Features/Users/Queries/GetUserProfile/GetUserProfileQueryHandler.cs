using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public GetUserProfileQueryHandler(IApplicationDbContext context, UserManager<IdentityUser> _userManager)
    {
        _context = context;
        this._userManager = _userManager;
    }

    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return Result.Failure<UserProfileDto>(new Error("User.NotFound", "Gebruiker niet gevonden."));
        }

        var roles = await _userManager.GetRolesAsync(user);
        var globalRole = roles.FirstOrDefault() ?? "User";

        var orgs = await _context.OrganizationUsers
            .Where(ou => ou.UserId == request.UserId)
            .Join(
                _context.Organizations,
                ou => ou.OrganizationId,
                o => o.Id,
                (ou, o) => new UserOrganizationDto(
                    o.Id,
                    o.Name,
                    ou.Role
                )
            )
            .ToListAsync(cancellationToken);

        var orgIds = orgs.Select(o => o.Id).ToList();

        var projects = await _context.Projects
            .Where(p => orgIds.Contains(p.OrganizationId))
            .Join(
                _context.Organizations,
                p => p.OrganizationId,
                o => o.Id,
                (p, o) => new UserProjectDto(
                    p.Id,
                    p.Name,
                    p.Color,
                    p.Icon,
                    o.Name
                )
            )
            .ToListAsync(cancellationToken);

        var profile = new UserProfileDto(
            user.Id,
            user.UserName ?? "Onbekende gebruiker",
            user.Email ?? string.Empty,
            globalRole,
            orgs,
            projects
        );

        return Result.Success(profile);
    }
}
