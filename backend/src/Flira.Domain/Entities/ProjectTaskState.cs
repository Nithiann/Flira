using System;

namespace Flira.Domain.Entities;

public class ProjectTaskState
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AllowedTransitionsJson { get; set; } = "[]";
    public bool IsInitial { get; set; }

    public Project? Project { get; set; }
}
