using System;

namespace Flira.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TaskItem? TaskItem { get; set; }
}
