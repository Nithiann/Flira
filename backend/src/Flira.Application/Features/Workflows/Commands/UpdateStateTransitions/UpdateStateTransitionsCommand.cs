using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Workflows.Commands.UpdateStateTransitions;

public record UpdateStateTransitionsCommand(
    Guid ProjectId,
    string StateName,
    List<string> AllowedTransitions) : IRequest<Result>;
