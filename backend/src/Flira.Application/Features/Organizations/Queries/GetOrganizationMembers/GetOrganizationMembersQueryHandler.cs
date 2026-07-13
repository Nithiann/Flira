using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Organizations.Queries.GetOrganizationMembers;

public class GetOrganizationMembersQueryHandler : IRequestHandler<GetOrganizationMembersQuery, Result<List<OrganizationMemberDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetOrganizationMembersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<OrganizationMemberDto>>> Handle(GetOrganizationMembersQuery request, CancellationToken cancellationToken)
    {
        var orgExists = await _context.Organizations.AnyAsync(o => o.Id == request.OrganizationId, cancellationToken);
        if (!orgExists)
        {
            return Result.Failure<List<OrganizationMemberDto>>(new Error("Organization.NotFound", "Organisatie niet gevonden."));
        }

        var members = await _context.OrganizationUsers
            .Where(ou => ou.OrganizationId == request.OrganizationId)
            .Join(_context.Users,
                ou => ou.UserId,
                u => u.Id,
                (ou, u) => new { ou.UserId, u.UserName, u.Email, ou.Role })
            .Select(m => new OrganizationMemberDto(
                m.UserId,
                m.UserName ?? "Onbekende gebruiker",
                m.Email ?? "",
                m.Role
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(members);
    }
}
