using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public record MarkAllNotificationsAsReadCommand(string UserId) : IRequest<Result>;
