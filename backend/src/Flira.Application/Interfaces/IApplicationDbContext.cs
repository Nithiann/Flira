using System.Threading;
using System.Threading.Tasks;
using Flira.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<IdentityUser> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Team> Teams { get; }
    DbSet<Project> Projects { get; }
    DbSet<Board> Boards { get; }
    DbSet<BoardColumn> BoardColumns { get; }
    DbSet<TaskItem> TaskItems { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<OrganizationUser> OrganizationUsers { get; }
    DbSet<TeamUser> TeamUsers { get; }
    DbSet<ProjectTaskState> ProjectTaskStates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
