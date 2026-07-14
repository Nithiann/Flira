using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Common.Events;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using MediatR;

namespace Flira.Application.Features.Tasks.EventHandlers;

public class CreateStatusChangeCommentHandler : INotificationHandler<TaskStatusUpdatedEvent>
{
    private readonly IApplicationDbContext _context;

    public CreateStatusChangeCommentHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(TaskStatusUpdatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.OldStatus.Equals(notification.NewStatus, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            TaskItemId = notification.TaskId,
            UserId = notification.UserId,
            Content = $"Status veranderd van {notification.OldStatus} naar {notification.NewStatus}",
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
