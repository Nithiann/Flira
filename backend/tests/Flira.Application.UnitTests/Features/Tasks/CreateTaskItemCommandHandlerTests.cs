using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Features.Tasks.Commands.CreateTask;
using Flira.Domain.Entities;
using Flira.Domain.Enums;
using Flira.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Flira.Application.UnitTests.Features.Tasks;

public class CreateTaskItemCommandHandlerTests : IDisposable
{
    private readonly FliraDbContext _context;
    private readonly Mock<IMediator> _mockMediator;

    public CreateTaskItemCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FliraDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FliraDbContext(options);
        _mockMediator = new Mock<IMediator>();
    }

    [Fact]
    public async Task Handle_Should_CreateTaskAndPublishEvent_WhenColumnExistsAndAssigneeProvided()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var board = new Board { Id = Guid.NewGuid(), ProjectId = projectId, Name = "Board" };
        var column = new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "Backlog", Board = board };
        var state = new ProjectTaskState { Id = Guid.NewGuid(), ProjectId = projectId, Name = "Backlog", IsInitial = true };

        _context.Boards.Add(board);
        _context.BoardColumns.Add(column);
        _context.ProjectTaskStates.Add(state);
        await _context.SaveChangesAsync();

        var handler = new CreateTaskItemCommandHandler(_context, _mockMediator.Object);
        var command = new CreateTaskItemCommand(
            column.Id,
            "New Task Title",
            "Task Description",
            TaskPriority.High,
            "assignee-id",
            "reporter-id",
            DateTime.UtcNow.AddDays(7),
            4.5m
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        
        var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == result.Value);
        Assert.NotNull(task);
        Assert.Equal("New Task Title", task.Title);
        Assert.Equal("Task Description", task.Description);
        Assert.Equal(TaskPriority.High, task.Priority);
        Assert.Equal("Backlog", task.Status);
        Assert.Equal("assignee-id", task.AssigneeId);

        _mockMediator.Verify(
            m => m.Publish(It.Is<Common.Events.TaskAssignedEvent>(
                e => e.TaskId == task.Id && e.AssigneeId == "assignee-id"), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenColumnDoesNotExist()
    {
        // Arrange
        var handler = new CreateTaskItemCommandHandler(_context, _mockMediator.Object);
        var command = new CreateTaskItemCommand(
            Guid.NewGuid(),
            "Title",
            "Desc",
            TaskPriority.Medium,
            null,
            null,
            null,
            null
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Column.NotFound", result.Error.Code);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
