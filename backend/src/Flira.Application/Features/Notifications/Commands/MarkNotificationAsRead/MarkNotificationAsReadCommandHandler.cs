using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public MarkNotificationAsReadCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken);

        if (notification == null)
        {
            return Result.Failure(new Error("Notification.NotFound", "Notificatie niet gevonden."));
        }

        if (notification.UserId != request.UserId)
        {
            return Result.Failure(new Error("Notification.Forbidden", "Je bent niet geautoriseerd om deze notificatie te wijzigen."));
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
