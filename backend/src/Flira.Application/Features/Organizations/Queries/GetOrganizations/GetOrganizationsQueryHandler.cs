using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Organizations.Queries.GetOrganizations;

public class GetOrganizationsQueryHandler : IRequestHandler<GetOrganizationsQuery, Result<List<OrganizationDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetOrganizationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<OrganizationDto>>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var orgs = await _context.OrganizationUsers
            .Where(ou => ou.UserId == request.UserId)
            .Select(ou => ou.Organization)
            .Where(o => o != null)
            .Select(o => new OrganizationDto(o!.Id, o.Name, o.Description))
            .ToListAsync(cancellationToken);

        return Result.Success(orgs);
    }
}
