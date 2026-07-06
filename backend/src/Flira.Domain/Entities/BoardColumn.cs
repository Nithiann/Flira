using System;
using System.Collections.Generic;

namespace Flira.Domain.Entities;

public class BoardColumn
{
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Board? Board { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
