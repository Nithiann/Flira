using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Columns.Commands.CreateColumn;

public class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateColumnCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateColumnCommand request, CancellationToken cancellationToken)
    {
        var board = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == request.BoardId, cancellationToken);
        if (board == null)
        {
            return Result.Failure<Guid>(new Error("Board.NotFound", "Bord niet gevonden."));
        }

        // Get highest position
        var maxPosition = await _context.BoardColumns
            .Where(c => c.BoardId == request.BoardId)
            .Select(c => (int?)c.Position)
            .MaxAsync(cancellationToken) ?? -1;

        var column = new BoardColumn
        {
            Id = Guid.NewGuid(),
            BoardId = request.BoardId,
            Name = request.Name,
            Position = maxPosition + 1,
            CreatedAt = DateTime.UtcNow
        };

        _context.BoardColumns.Add(column);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(column.Id);
    }
}
