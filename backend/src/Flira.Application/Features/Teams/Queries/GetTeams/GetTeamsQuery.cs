using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Teams.Queries.GetTeams;

public record GetTeamsQuery(Guid OrganizationId) : IRequest<Result<List<TeamDto>>>;

public record TeamDto(Guid Id, Guid OrganizationId, string Name, string Description);
