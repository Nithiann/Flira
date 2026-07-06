using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Domain.States;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Flira.Persistence;

public class FliraDbContext : IdentityDbContext<IdentityUser>, IApplicationDbContext
{
    public FliraDbContext(DbContextOptions<FliraDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<BoardColumn> BoardColumns => Set<BoardColumn>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OrganizationUser> OrganizationUsers => Set<OrganizationUser>();
    public DbSet<TeamUser> TeamUsers => Set<TeamUser>();
    public DbSet<ProjectTaskState> ProjectTaskStates => Set<ProjectTaskState>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure relations and keys
        builder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        builder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Organization)
                  .WithMany(o => o.Teams)
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.HasOne(e => e.Organization)
                  .WithMany(o => o.Projects)
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        builder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Project)
                  .WithMany(p => p.Boards)
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<BoardColumn>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Board)
                  .WithMany(b => b.Columns)
                  .HasForeignKey(e => e.BoardId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status)
                  .IsRequired()
                  .HasMaxLength(50);
            entity.HasOne(e => e.BoardColumn)
                  .WithMany(c => c.Tasks)
                  .HasForeignKey(e => e.BoardColumnId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        builder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.HasOne(e => e.TaskItem)
                  .WithMany(t => t.Comments)
                  .HasForeignKey(e => e.TaskItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileUrl).IsRequired();
            entity.HasOne(e => e.TaskItem)
                  .WithMany(t => t.Attachments)
                  .HasForeignKey(e => e.TaskItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Content).IsRequired();
        });

        builder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(256);
        });

        builder.Entity<OrganizationUser>(entity =>
        {
            entity.HasKey(ou => new { ou.OrganizationId, ou.UserId });
            entity.HasOne(ou => ou.Organization)
                  .WithMany()
                  .HasForeignKey(ou => ou.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TeamUser>(entity =>
        {
            entity.HasKey(tu => new { tu.TeamId, tu.UserId });
            entity.HasOne(tu => tu.Team)
                  .WithMany()
                  .HasForeignKey(tu => tu.TeamId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProjectTaskState>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AllowedTransitionsJson).IsRequired();
            entity.HasIndex(e => new { e.ProjectId, e.Name }).IsUnique();
            entity.HasOne(e => e.Project)
                  .WithMany()
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed default Identity Roles
        var adminRoleId = "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a01";
        var managerRoleId = "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a02";
        var userRoleId = "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a03";

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = managerRoleId, Name = "Manager", NormalizedName = "MANAGER" },
            new IdentityRole { Id = userRoleId, Name = "User", NormalizedName = "USER" }
        );
    }
}
