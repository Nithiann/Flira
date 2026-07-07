using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Features.Dashboard.Queries.GetDashboardStats;
using Flira.Domain.Entities;
using Flira.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Flira.Application.UnitTests.Features.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandlerTests : IDisposable
{
    private readonly FliraDbContext _context;

    public GetDashboardStatsQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FliraDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FliraDbContext(options);
    }

    [Fact]
    public async Task Handle_Should_CalculateStatsCorrectly_WhenProjectHasTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var board = new Board { Id = Guid.NewGuid(), ProjectId = projectId, Name = "Main Board", Project = project };
        var column = new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "Todo", Board = board };
        
        var taskOpen = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Open Task",
            BoardColumnId = column.Id,
            BoardColumn = column,
            Status = "Todo",
            EstimatedHours = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };
        var taskClosed = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Closed Task",
            BoardColumnId = column.Id,
            BoardColumn = column,
            Status = "Done",
            CompletedAt = DateTime.UtcNow.AddDays(-2),
            EstimatedHours = 8,
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        };

        _context.Projects.Add(project);
        _context.Boards.Add(board);
        _context.BoardColumns.Add(column);
        _context.TaskItems.AddRange(taskOpen, taskClosed);
        await _context.SaveChangesAsync();

        var handler = new GetDashboardStatsQueryHandler(_context);
        var query = new GetDashboardStatsQuery(board.ProjectId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.OpenTasksCount);
        Assert.Equal(1, result.Value.ClosedTasksCount);
        Assert.Equal(8, result.Value.TotalCompletedHours);
        
        Assert.Equal(30, result.Value.BurndownData.Count);
        
        // Burndown remaining tasks checks:
        // Before taskOpen and taskClosed were created (30 days ago), remaining count should be 0.
        // On date of taskClosed creation (-15 days) up to before taskOpen creation (-10 days), remaining should be 1.
        // After taskOpen creation (-10 days) up to before taskClosed completion (-2 days), remaining should be 2.
        // After taskClosed completion (-2 days), remaining should be 1.
        var todayBurndown = result.Value.BurndownData.Last();
        Assert.Equal(1, todayBurndown.RemainingTasksCount);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
