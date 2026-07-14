using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Organizations.Queries.GetOrganizationMembers;

public record GetOrganizationMembersQuery(Guid OrganizationId) : IRequest<Result<List<OrganizationMemberDto>>>;

public record OrganizationMemberDto(
    string UserId,
    string Username,
    string Email,
    string Role);
