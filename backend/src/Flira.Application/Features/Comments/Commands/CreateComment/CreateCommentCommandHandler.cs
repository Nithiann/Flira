using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Common.Events;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMediator _mediator;

    public CreateCommentCommandHandler(
        IApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        IMediator mediator)
    {
        _context = context;
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .Include(t => t.BoardColumn)
                .ThenInclude(c => c!.Board)
            .FirstOrDefaultAsync(t => t.Id == request.TaskItemId, cancellationToken);

        if (task == null)
        {
            return Result.Failure<Guid>(new Error("Task.NotFound", "Taak niet gevonden."));
        }

        var author = await _userManager.FindByIdAsync(request.UserId);
        if (author == null)
        {
            return Result.Failure<Guid>(new Error("User.NotFound", "Auteur niet gevonden."));
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            TaskItemId = request.TaskItemId,
            UserId = request.UserId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        // Regex parser to find @username mentions
        var matches = Regex.Matches(request.Content, @"\B@([a-zA-Z0-9_\-\.]+)");
        var boardId = task.BoardColumn?.BoardId ?? Guid.Empty;
        var authorName = author.UserName ?? author.Email ?? "Onbekende gebruiker";

        foreach (Match match in matches)
        {
            var username = match.Groups[1].Value;
            var mentionedUser = await _userManager.FindByNameAsync(username);
            if (mentionedUser != null && mentionedUser.Id != request.UserId)
            {
                // Trigger a UserMentionedEvent
                await _mediator.Publish(new UserMentionedEvent(
                    task.Id,
                    boardId,
                    task.Title,
                    mentionedUser.Id,
                    authorName,
                    request.Content
                ), cancellationToken);
            }
        }

        return Result.Success(comment.Id);
    }
}
