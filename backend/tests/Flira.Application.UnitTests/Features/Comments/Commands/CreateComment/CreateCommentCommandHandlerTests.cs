using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Common.Events;
using Flira.Application.Features.Comments.Commands.CreateComment;
using Flira.Domain.Entities;
using Flira.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Flira.Application.UnitTests.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandHandlerTests : IDisposable
{
    private readonly FliraDbContext _context;
    private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
    private readonly Mock<IMediator> _mockMediator;
    private readonly List<IdentityUser> _usersList;

    public CreateCommentCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FliraDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FliraDbContext(options);
        _usersList = new List<IdentityUser>();

        // Set up Mock UserManager
        var store = new Mock<IUserStore<IdentityUser>>();
        _mockUserManager = new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => _usersList.FirstOrDefault(u => u.Id == id));

        _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((string name) => _usersList.FirstOrDefault(u => u.UserName == name));

        _mockMediator = new Mock<IMediator>();
    }

    [Fact]
    public async Task Handle_Should_CreateComment_And_PublishMentionEvent_When_CommentHasValidMention()
    {
        // Arrange
        var author = new IdentityUser { Id = "author-id", UserName = "author_user", Email = "author@flira.com" };
        var mentioned = new IdentityUser { Id = "mentioned-id", UserName = "john_doe", Email = "john@flira.com" };
        _usersList.Add(author);
        _usersList.Add(mentioned);

        var board = new Board { Id = Guid.NewGuid(), Name = "Main Board" };
        var column = new BoardColumn { Id = Guid.NewGuid(), BoardId = board.Id, Name = "Todo", Board = board };
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            BoardColumnId = column.Id,
            BoardColumn = column,
            Status = "Todo"
        };

        _context.Boards.Add(board);
        _context.BoardColumns.Add(column);
        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();

        var handler = new CreateCommentCommandHandler(_context, _mockUserManager.Object, _mockMediator.Object);
        var command = new CreateCommentCommand(task.Id, author.Id, "Hello @john_doe, please check this.");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        var dbComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == result.Value);
        Assert.NotNull(dbComment);
        Assert.Equal("Hello @john_doe, please check this.", dbComment.Content);
        Assert.Equal(task.Id, dbComment.TaskItemId);
        Assert.Equal(author.Id, dbComment.UserId);

        _mockMediator.Verify(m => m.Publish(
            It.Is<UserMentionedEvent>(e => 
                e.TaskId == task.Id &&
                e.BoardId == board.Id &&
                e.MentionedUserId == mentioned.Id &&
                e.CommentAuthorName == author.UserName &&
                e.CommentContent == command.Content
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TaskDoesNotExist()
    {
        // Arrange
        var handler = new CreateCommentCommandHandler(_context, _mockUserManager.Object, _mockMediator.Object);
        var command = new CreateCommentCommand(Guid.NewGuid(), "author-id", "Some comment content");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Task.NotFound", result.Error.Code);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
