using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Workflows.Commands.CreateCustomState;

public record CreateCustomStateCommand(
    Guid ProjectId,
    string Name,
    List<string> AllowedTransitions) : IRequest<Result>;
