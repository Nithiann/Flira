using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(string UserId) : IRequest<Result<List<NotificationDto>>>;

public record NotificationDto(
    Guid Id,
    string UserId,
    string Title,
    string Content,
    string Link,
    bool IsRead,
    DateTime CreatedAt);
