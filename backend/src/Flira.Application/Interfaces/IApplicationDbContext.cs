using System.Threading;
using System.Threading.Tasks;
using Flira.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Interfaces;

public interface IApplicationDbContext
{
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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
