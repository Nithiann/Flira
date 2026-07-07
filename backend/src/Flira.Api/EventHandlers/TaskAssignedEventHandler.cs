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

public class TaskAssignedEventHandler : INotificationHandler<TaskAssignedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public TaskAssignedEventHandler(
        IApplicationDbContext context,
        IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task Handle(TaskAssignedEvent notification, CancellationToken cancellationToken)
    {
        // Avoid sending notification to oneself if they assign a task to themselves
        if (notification.AssigneeId == notification.AssignedById)
        {
            return;
        }

        var dbNotification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = notification.AssigneeId,
            Title = "Nieuwe taak toegewezen",
            Content = $"De taak '{notification.TaskTitle}' is aan jou toegewezen.",
            Link = $"/projects/boards/{notification.BoardId}/tasks/{notification.TaskId}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(dbNotification);
        await _context.SaveChangesAsync(cancellationToken);

        await _hubContext.Clients.User(notification.AssigneeId)
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
