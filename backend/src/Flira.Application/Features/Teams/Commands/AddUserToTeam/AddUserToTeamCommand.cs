using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Teams.Commands.AddUserToTeam;

public record AddUserToTeamCommand(Guid TeamId, string Email) : IRequest<Result>;
