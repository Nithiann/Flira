using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand(Guid NotificationId, string UserId) : IRequest<Result>;
