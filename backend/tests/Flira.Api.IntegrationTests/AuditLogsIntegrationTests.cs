using System;
using System.Linq;
using System.Threading.Tasks;
using Flira.Domain.Entities;
using Flira.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Flira.Api.IntegrationTests;

public class AuditLogsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuditLogsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SaveChangesAsync_Should_RecordAuditLogs_WhenEntitiesAreMutated()
    {
        // Arrange
        _factory.CurrentUserId = "audited-user-id";
        
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FliraDbContext>();

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Audit Testing Org",
            Description = "Checking EF Core change logging"
        };

        // Act - Insert
        context.Organizations.Add(organization);
        await context.SaveChangesAsync();

        // Assert - Insert Logged
        var logsAfterInsert = await context.AuditLogs
            .Where(l => l.EntityName == "Organizations" || l.EntityName == "Organization")
            .ToListAsync();

        Assert.NotEmpty(logsAfterInsert);
        var insertLog = logsAfterInsert.FirstOrDefault(l => l.Action == "Added");
        Assert.NotNull(insertLog);
        Assert.Equal(_factory.CurrentUserId, insertLog.UserId);
        Assert.NotNull(insertLog.NewValues);
        Assert.Contains("Audit Testing Org", insertLog.NewValues);

        // Act - Update
        organization.Name = "Audit Testing Org - Updated Name";
        await context.SaveChangesAsync();

        // Assert - Update Logged
        var logsAfterUpdate = await context.AuditLogs
            .Where(l => l.EntityName == "Organizations" || l.EntityName == "Organization")
            .ToListAsync();

        var updateLog = logsAfterUpdate.FirstOrDefault(l => l.Action == "Modified");
        Assert.NotNull(updateLog);
        Assert.Equal(_factory.CurrentUserId, updateLog.UserId);
        Assert.NotNull(updateLog.OldValues);
        Assert.NotNull(updateLog.NewValues);
        Assert.Contains("Audit Testing Org", updateLog.OldValues);
        Assert.Contains("Audit Testing Org - Updated Name", updateLog.NewValues);
    }
}
