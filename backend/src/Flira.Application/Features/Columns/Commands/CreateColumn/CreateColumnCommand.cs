using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Columns.Commands.CreateColumn;

public record CreateColumnCommand(Guid BoardId, string Name) : IRequest<Result<Guid>>;
