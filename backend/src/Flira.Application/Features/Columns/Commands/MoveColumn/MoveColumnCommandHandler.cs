using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Columns.Commands.MoveColumn;

public class MoveColumnCommandHandler : IRequestHandler<MoveColumnCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public MoveColumnCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(MoveColumnCommand request, CancellationToken cancellationToken)
    {
        var column = await _context.BoardColumns
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId, cancellationToken);
        if (column == null)
        {
            return Result.Failure(new Error("Column.NotFound", "Kolom niet gevonden."));
        }

        var oldPosition = column.Position;
        var newPosition = request.NewPosition;
        var boardId = column.BoardId;

        if (oldPosition == newPosition)
        {
            return Result.Success();
        }

        var maxPosition = await _context.BoardColumns
            .Where(c => c.BoardId == boardId)
            .CountAsync(cancellationToken) - 1;

        if (newPosition < 0) newPosition = 0;
        if (newPosition > maxPosition) newPosition = maxPosition;

        if (newPosition < oldPosition)
        {
            var columnsToShift = await _context.BoardColumns
                .Where(c => c.BoardId == boardId && c.Position >= newPosition && c.Position < oldPosition)
                .ToListAsync(cancellationToken);

            foreach (var col in columnsToShift)
            {
                col.Position += 1;
            }
        }
        else
        {
            var columnsToShift = await _context.BoardColumns
                .Where(c => c.BoardId == boardId && c.Position > oldPosition && c.Position <= newPosition)
                .ToListAsync(cancellationToken);

            foreach (var col in columnsToShift)
            {
                col.Position -= 1;
            }
        }

        column.Position = newPosition;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
