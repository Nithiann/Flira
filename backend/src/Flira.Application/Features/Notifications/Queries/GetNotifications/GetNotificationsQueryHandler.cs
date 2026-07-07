using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<List<NotificationDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetNotificationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == request.UserId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(
                n.Id,
                n.UserId,
                n.Title,
                n.Content,
                n.Link,
                n.IsRead,
                n.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(notifications);
    }
}
