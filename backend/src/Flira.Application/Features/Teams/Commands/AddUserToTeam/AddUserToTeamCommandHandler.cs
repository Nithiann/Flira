using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Teams.Commands.AddUserToTeam;

public class AddUserToTeamCommandHandler : IRequestHandler<AddUserToTeamCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AddUserToTeamCommandHandler(IApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result> Handle(AddUserToTeamCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure(new Error("User.NotFound", "Gebruiker niet gevonden."));
        }

        var team = await _context.Teams
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken);
        if (team == null)
        {
            return Result.Failure(new Error("Team.NotFound", "Team niet gevonden."));
        }

        var isOrgMember = await _context.OrganizationUsers
            .AnyAsync(ou => ou.OrganizationId == team.OrganizationId && ou.UserId == user.Id, cancellationToken);
        if (!isOrgMember)
        {
            return Result.Failure(new Error("Team.UserNotOrgMember", "Gebruiker moet eerst aan de organisatie worden toegevoegd."));
        }

        var alreadyInTeam = await _context.TeamUsers
            .AnyAsync(tu => tu.TeamId == request.TeamId && tu.UserId == user.Id, cancellationToken);
        if (alreadyInTeam)
        {
            return Result.Failure(new Error("Team.UserAlreadyInTeam", "Gebruiker is al lid van dit team."));
        }

        var teamUser = new TeamUser
        {
            TeamId = request.TeamId,
            UserId = user.Id
        };

        _context.TeamUsers.Add(teamUser);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
