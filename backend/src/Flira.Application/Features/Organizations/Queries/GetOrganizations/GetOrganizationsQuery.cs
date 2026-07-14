using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Organizations.Queries.GetOrganizations;

public record GetOrganizationsQuery(string UserId) : IRequest<Result<List<OrganizationDto>>>;

public record OrganizationDto(Guid Id, string Name, string Description, string Role);
