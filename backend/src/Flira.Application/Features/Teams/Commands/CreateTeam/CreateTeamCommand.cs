using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Teams.Commands.CreateTeam;

public record CreateTeamCommand(Guid OrganizationId, string Name, string Description) : IRequest<Result<Guid>>;
