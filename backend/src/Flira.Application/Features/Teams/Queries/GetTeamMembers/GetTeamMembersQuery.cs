using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Teams.Queries.GetTeamMembers;

public record GetTeamMembersQuery(Guid TeamId) : IRequest<Result<List<TeamMemberDto>>>;

public record TeamMemberDto(string Id, string UserName, string Email, string Role);
