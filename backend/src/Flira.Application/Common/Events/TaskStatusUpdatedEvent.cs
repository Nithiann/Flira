using System;
using MediatR;

namespace Flira.Application.Common.Events;

public record TaskStatusUpdatedEvent(
    Guid TaskId,
    Guid BoardId,
    string OldStatus,
    string NewStatus,
    Guid NewBoardColumnId,
    string UserId) : INotification;
