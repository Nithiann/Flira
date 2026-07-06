using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Columns.Commands.DeleteColumn;

public record DeleteColumnCommand(Guid ColumnId) : IRequest<Result>;
