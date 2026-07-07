using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Features.Search.Queries.SearchTasks;
using Flira.Domain.Entities;
using Flira.Domain.Enums;
using Flira.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Flira.Application.UnitTests.Features.Search.Queries.SearchTasks;

public class SearchTasksQueryHandlerTests : IDisposable
{
    private readonly FliraDbContext _context;

    public SearchTasksQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FliraDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FliraDbContext(options);
    }

    [Fact]
    public async Task Handle_Should_FilterTasksCorrectly_BasedOnSearchTermAndProjectId()
    {
        // Arrange
        var board = new Board { Id = Guid.NewGuid(), Name = "Main Board" };
        var column = new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "Todo", Board = board };
        
        var task1 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Implement login functionality",
            Description = "Use JWT tokens",
            BoardColumnId = column.Id,
            BoardColumn = column,
            Status = "Todo",
            Priority = TaskPriority.High,
            Labels = "Auth,Security"
        };
        var task2 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Create database indexes",
            Description = "Make search fast",
            BoardColumnId = column.Id,
            BoardColumn = column,
            Status = "Todo",
            Priority = TaskPriority.Medium,
            Labels = "Database"
        };

        _context.Boards.Add(board);
        _context.BoardColumns.Add(column);
        _context.TaskItems.AddRange(task1, task2);
        await _context.SaveChangesAsync();

        var handler = new SearchTasksQueryHandler(_context);

        // Act - Search term matches title of task1
        var query = new SearchTasksQuery("login", board.ProjectId, null, null, null, null, 1, 10, "Title", "asc");
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Items);
        Assert.Equal(task1.Id, result.Value.Items[0].Id);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Equal(1, result.Value.TotalPages);
    }

    [Fact]
    public async Task Handle_Should_FilterTasksCorrectly_BasedOnLabels()
    {
        // Arrange
        var board = new Board { Id = Guid.NewGuid(), Name = "Main Board" };
        var column = new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "Todo", Board = board };
        
        var task1 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 1",
            BoardColumnId = column.Id,
            BoardColumn = column,
            Labels = "Frontend,UI"
        };
        var task2 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 2",
            BoardColumnId = column.Id,
            BoardColumn = column,
            Labels = "Backend,API"
        };

        _context.Boards.Add(board);
        _context.BoardColumns.Add(column);
        _context.TaskItems.AddRange(task1, task2);
        await _context.SaveChangesAsync();

        var handler = new SearchTasksQueryHandler(_context);

        // Act - Search for labels containing "frontend"
        var query = new SearchTasksQuery(null, board.ProjectId, null, null, null, "Frontend", 1, 10, "Title", "asc");
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Items);
        Assert.Equal(task1.Id, result.Value.Items[0].Id);
    }

    [Fact]
    public async Task Handle_Should_PaginateResultsCorrectly()
    {
        // Arrange
        var board = new Board { Id = Guid.NewGuid(), Name = "Main Board" };
        var column = new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "Todo", Board = board };
        
        for (int i = 1; i <= 15; i++)
        {
            _context.TaskItems.Add(new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = $"Task {i:D2}", // Task 01, Task 02...
                BoardColumnId = column.Id,
                BoardColumn = column
            });
        }

        _context.Boards.Add(board);
        _context.BoardColumns.Add(column);
        await _context.SaveChangesAsync();

        var handler = new SearchTasksQueryHandler(_context);

        // Act - Page 2, PageSize 5 (should return items 6-10, alphabetical sort by Title)
        var query = new SearchTasksQuery(null, board.ProjectId, null, null, null, null, 2, 5, "Title", "asc");
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value.Items.Count);
        Assert.Equal(15, result.Value.TotalCount);
        Assert.Equal(3, result.Value.TotalPages);
        Assert.Equal("Task 06", result.Value.Items[0].Title);
        Assert.Equal("Task 10", result.Value.Items[4].Title);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
