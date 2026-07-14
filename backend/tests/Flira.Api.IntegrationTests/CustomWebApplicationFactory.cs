using System;
using System.Linq;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Application.Security;
using Flira.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.PostgreSql;
using Xunit;

namespace Flira.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:15-alpine")
        .Build();

    public string CurrentUserId { get; set; } = "test-user-id";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DB context registration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<FliraDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Add transient database pointing to Testcontainer
            services.AddDbContext<FliraDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString())
                       .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

            // Replace ICurrentUserService with Mock to control active user context in tests
            var currentUserDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ICurrentUserService));

            if (currentUserDescriptor != null)
            {
                services.Remove(currentUserDescriptor);
            }

            var mockCurrentUserService = new Mock<ICurrentUserService>();
            mockCurrentUserService.Setup(m => m.UserId).Returns(() => CurrentUserId);
            services.AddScoped<ICurrentUserService>(_ => mockCurrentUserService.Object);

            // Replace IPermissionService with Mock to allow bypassing security policies in integration tests
            var permissionServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IPermissionService));

            if (permissionServiceDescriptor != null)
            {
                services.Remove(permissionServiceDescriptor);
            }

            var mockPermissionService = new Mock<IPermissionService>();
            mockPermissionService.Setup(m => m.HasPermissionAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            services.AddScoped<IPermissionService>(_ => mockPermissionService.Object);
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Run migrations
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FliraDbContext>();
        await context.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
