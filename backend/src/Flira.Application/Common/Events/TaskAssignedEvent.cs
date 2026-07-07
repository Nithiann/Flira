using System;
using MediatR;

namespace Flira.Application.Common.Events;

public record TaskAssignedEvent(Guid TaskId, Guid BoardId, string TaskTitle, string AssigneeId, string AssignedById) : INotification;
