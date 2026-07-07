using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Api.Hubs;
using Flira.Application.Common.Events;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Flira.Api.EventHandlers;

public class UserMentionedEventHandler : INotificationHandler<UserMentionedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public UserMentionedEventHandler(
        IApplicationDbContext context,
        IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task Handle(UserMentionedEvent notification, CancellationToken cancellationToken)
    {
        var displayComment = notification.CommentContent.Length > 60 
            ? notification.CommentContent.Substring(0, 57) + "..." 
            : notification.CommentContent;

        var dbNotification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = notification.MentionedUserId,
            Title = "Genoemd in reactie",
            Content = $"{notification.CommentAuthorName} noemde jou in de taak '{notification.TaskTitle}': \"{displayComment}\"",
            Link = $"/projects/boards/{notification.BoardId}/tasks/{notification.TaskId}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(dbNotification);
        await _context.SaveChangesAsync(cancellationToken);

        await _hubContext.Clients.User(notification.MentionedUserId)
            .SendAsync("ReceiveNotification", new
            {
                dbNotification.Id,
                dbNotification.UserId,
                dbNotification.Title,
                dbNotification.Content,
                dbNotification.Link,
                dbNotification.IsRead,
                dbNotification.CreatedAt
            }, cancellationToken);
    }
}
