using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Columns.Commands.UpdateColumn;

public class UpdateColumnCommandHandler : IRequestHandler<UpdateColumnCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateColumnCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateColumnCommand request, CancellationToken cancellationToken)
    {
        var column = await _context.BoardColumns
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId, cancellationToken);
        if (column == null)
        {
            return Result.Failure(new Error("Column.NotFound", "Kolom niet gevonden."));
        }

        column.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
