using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Teams.Queries.GetTeamMembers;

public class GetTeamMembersQueryHandler : IRequestHandler<GetTeamMembersQuery, Result<List<TeamMemberDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetTeamMembersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TeamMemberDto>>> Handle(GetTeamMembersQuery request, CancellationToken cancellationToken)
    {
        var team = await _context.Teams
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken);
        
        if (team == null)
        {
            return Result.Failure<List<TeamMemberDto>>(new Error("Team.NotFound", "Team niet gevonden."));
        }

        var members = await _context.TeamUsers
            .Where(tu => tu.TeamId == request.TeamId)
            .Join(_context.Users,
                tu => tu.UserId,
                u => u.Id,
                (tu, u) => new { tu.UserId, u.UserName, u.Email })
            .GroupJoin(_context.OrganizationUsers.Where(ou => ou.OrganizationId == team.OrganizationId),
                x => x.UserId,
                ou => ou.UserId,
                (x, ous) => new { x.UserId, x.UserName, x.Email, Role = ous.Select(ou => ou.Role).FirstOrDefault() ?? "Member" })
            .Select(m => new TeamMemberDto(m.UserId, m.UserName ?? "", m.Email ?? "", m.Role))
            .ToListAsync(cancellationToken);

        return Result.Success(members);
    }
}
