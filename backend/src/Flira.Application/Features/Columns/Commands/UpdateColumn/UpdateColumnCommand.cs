using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Columns.Commands.UpdateColumn;

public record UpdateColumnCommand(Guid ColumnId, string Name) : IRequest<Result>;
