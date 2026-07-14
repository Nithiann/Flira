using System;
using MediatR;

namespace Flira.Application.Common.Events;

public record UserMentionedEvent(Guid TaskId, Guid BoardId, string TaskTitle, string MentionedUserId, string CommentAuthorName, string CommentContent) : INotification;
