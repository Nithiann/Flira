using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Columns.Commands.MoveColumn;

public record MoveColumnCommand(Guid ColumnId, int NewPosition) : IRequest<Result>;
