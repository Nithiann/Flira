using System;
using System.Collections.Generic;
using Flira.Domain.Enums;

namespace Flira.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid BoardColumnId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Backlog;
    public string? AssigneeId { get; set; }
    public string? ReporterId { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    public BoardColumn? BoardColumn { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
