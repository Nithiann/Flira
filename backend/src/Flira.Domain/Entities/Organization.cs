using System;
using System.Collections.Generic;

namespace Flira.Domain.Entities;

public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Team> Teams { get; set; } = new List<Team>();
}
