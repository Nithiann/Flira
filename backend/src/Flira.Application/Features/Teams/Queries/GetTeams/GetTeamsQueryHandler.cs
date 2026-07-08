using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Teams.Queries.GetTeams;

public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, Result<List<TeamDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetTeamsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TeamDto>>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
    {
        var teams = await _context.Teams
            .Where(t => t.OrganizationId == request.OrganizationId)
            .Select(t => new TeamDto(t.Id, t.OrganizationId, t.Name, t.Description))
            .ToListAsync(cancellationToken);

        return Result.Success(teams);
    }
}
