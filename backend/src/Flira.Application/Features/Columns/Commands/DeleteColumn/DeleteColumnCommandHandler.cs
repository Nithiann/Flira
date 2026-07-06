using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Columns.Commands.DeleteColumn;

public class DeleteColumnCommandHandler : IRequestHandler<DeleteColumnCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteColumnCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteColumnCommand request, CancellationToken cancellationToken)
    {
        var column = await _context.BoardColumns
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId, cancellationToken);
        if (column == null)
        {
            return Result.Failure(new Error("Column.NotFound", "Kolom niet gevonden."));
        }

        var deletedPosition = column.Position;
        var boardId = column.BoardId;

        _context.BoardColumns.Remove(column);

        // Shift subsequent positions down by 1
        var subsequentColumns = await _context.BoardColumns
            .Where(c => c.BoardId == boardId && c.Position > deletedPosition)
            .ToListAsync(cancellationToken);

        foreach (var subCol in subsequentColumns)
        {
            subCol.Position -= 1;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
