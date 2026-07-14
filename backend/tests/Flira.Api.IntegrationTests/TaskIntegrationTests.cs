using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Flira.Api.Controllers;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Domain.Enums;
using Flira.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Flira.Api.IntegrationTests;

public class TaskIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TaskIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTask_Should_CreateTaskInDatabase_And_ReturnTaskId_When_Authorized()
    {
        // Arrange - Seed parent entities
        var orgId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var boardId = Guid.NewGuid();
        var columnId = Guid.NewGuid();

        string token;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<FliraDbContext>();
            var jwtGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
            token = jwtGenerator.GenerateToken("test-user-id", "test@flira.com", "test_user", new List<string> { "User" });

            var org = new Organization { Id = orgId, Name = "Test Org", Description = "Testing Tasks" };
            var project = new Project { Id = projectId, OrganizationId = orgId, Name = "Test Project" };
            var board = new Board { Id = boardId, ProjectId = projectId, Name = "Test Board" };
            var column = new BoardColumn { Id = columnId, BoardId = boardId, Name = "To Do" };
            var state = new ProjectTaskState { Id = Guid.NewGuid(), ProjectId = projectId, Name = "To Do", IsInitial = true };

            context.Organizations.Add(org);
            context.Projects.Add(project);
            context.Boards.Add(board);
            context.BoardColumns.Add(column);
            context.ProjectTaskStates.Add(state);

            await context.SaveChangesAsync();
        }

        // Add headers for authorization and context
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _client.DefaultRequestHeaders.Add("X-Organization-Id", orgId.ToString());

        var model = new CreateTaskModel(
            columnId,
            "Integration Test Task",
            "This task was created by integration tests.",
            TaskPriority.High,
            "test-user-id",
            "test-user-id",
            DateTime.UtcNow.AddDays(3),
            2.5m
        );

        // Act
        var response = await _client.PostAsJsonAsync("api/tasks", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateTaskResponseDto>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.TaskId);

        // Verify database persistence
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<FliraDbContext>();
            var taskInDb = await context.TaskItems.FirstOrDefaultAsync(t => t.Id == result.TaskId);
            
            Assert.NotNull(taskInDb);
            Assert.Equal("Integration Test Task", taskInDb.Title);
            Assert.Equal("This task was created by integration tests.", taskInDb.Description);
            Assert.Equal("To Do", taskInDb.Status);
            Assert.Equal(TaskPriority.High, taskInDb.Priority);
            Assert.Equal(2.5m, taskInDb.EstimatedHours);
        }
    }

    [Fact]
    public async Task CreateTask_Should_ReturnBadRequest_When_OrganizationIdHeaderIsMissing()
    {
        // Arrange
        _client.DefaultRequestHeaders.Clear();
        var model = new CreateTaskModel(
            Guid.NewGuid(),
            "Task Title",
            "Desc",
            TaskPriority.Medium,
            null,
            null,
            null,
            null
        );

        // Act
        var response = await _client.PostAsJsonAsync("api/tasks", model);

        // Assert
        // Missing X-Organization-Id header should cause the HasPermission authorization handler
        // to fail, returning 403 Forbidden or 401 Unauthorized depending on route authorization.
        Assert.True(response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized);
    }

    private class CreateTaskResponseDto
    {
        public Guid TaskId { get; set; }
    }
}
