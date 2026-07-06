using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Organizations.Commands.CreateOrganization;

public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Organizations.Add(organization);

        var organizationUser = new OrganizationUser
        {
            OrganizationId = organization.Id,
            UserId = request.CreatorUserId,
            Role = "Admin"
        };

        _context.OrganizationUsers.Add(organizationUser);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(organization.Id);
    }
}
