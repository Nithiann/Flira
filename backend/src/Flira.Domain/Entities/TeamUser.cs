using System;

namespace Flira.Domain.Entities;

public class TeamUser
{
    public Guid TeamId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public Team? Team { get; set; }
}
