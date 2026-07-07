using System;
using MediatR;

namespace Flira.Application.Common.Events;

public record TaskStatusUpdatedEvent(Guid TaskId, Guid BoardId, string NewStatus, Guid NewBoardColumnId) : INotification;
