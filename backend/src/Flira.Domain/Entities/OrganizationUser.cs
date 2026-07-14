using System;

namespace Flira.Domain.Entities;

public class OrganizationUser
{
    public Guid OrganizationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Admin, Manager, Member, Guest

    public Organization? Organization { get; set; }
}
