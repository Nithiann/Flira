using System;
using System.Collections.Generic;

namespace Flira.Domain.Entities;

public class Board
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project? Project { get; set; }
    public ICollection<BoardColumn> Columns { get; set; } = new List<BoardColumn>();
}
