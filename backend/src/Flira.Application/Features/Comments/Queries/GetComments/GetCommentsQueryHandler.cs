using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Comments.Queries.GetComments;

public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, Result<List<CommentDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCommentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CommentDto>>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var taskExists = await _context.TaskItems.AnyAsync(t => t.Id == request.TaskItemId, cancellationToken);
        if (!taskExists)
        {
            return Result.Failure<List<CommentDto>>(new Error("Task.NotFound", "Taak niet gevonden."));
        }

        var comments = await _context.Comments
            .Where(c => c.TaskItemId == request.TaskItemId)
            .Join(
                _context.Users,
                comment => comment.UserId,
                user => user.Id,
                (comment, user) => new { comment, user }
            )
            .OrderBy(x => x.comment.CreatedAt)
            .Select(x => new CommentDto(
                x.comment.Id,
                x.comment.TaskItemId,
                x.comment.UserId,
                x.user.UserName ?? "Onbekende gebruiker",
                x.comment.Content,
                x.comment.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(comments);
    }
}
