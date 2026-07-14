using System.Threading;
using System.Threading.Tasks;
using Flira.Api.Hubs;
using Flira.Application.Common.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Flira.Api.EventHandlers;

public class TaskStatusUpdatedEventHandler : INotificationHandler<TaskStatusUpdatedEvent>
{
    private readonly IHubContext<BoardHub> _hubContext;

    public TaskStatusUpdatedEventHandler(IHubContext<BoardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(TaskStatusUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Group($"board_{notification.BoardId}")
            .SendAsync("ReceiveTaskStatusUpdate", new
            {
                notification.TaskId,
                notification.NewStatus,
                notification.NewBoardColumnId
            }, cancellationToken);
    }
}
