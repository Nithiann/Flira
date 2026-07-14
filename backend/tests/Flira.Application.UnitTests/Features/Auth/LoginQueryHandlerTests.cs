using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Features.Auth;
using Flira.Application.Features.Auth.Queries.Login;
using Flira.Application.Interfaces;
using Flira.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Flira.Application.UnitTests.Features.Auth;

public class LoginQueryHandlerTests : IDisposable
{
    private readonly FliraDbContext _context;
    private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;

    public LoginQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FliraDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FliraDbContext(options);

        var store = new Mock<IUserStore<IdentityUser>>();
        _mockUserManager = new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
    }

    [Fact]
    public async Task Handle_Should_ReturnAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new IdentityUser { Id = "user-id", Email = "test@flira.com", UserName = "test_user" };
        _mockUserManager.Setup(m => m.FindByEmailAsync("test@flira.com"))
            .ReturnsAsync(user);

        _mockUserManager.Setup(m => m.CheckPasswordAsync(user, "Password123!"))
            .ReturnsAsync(true);

        _mockUserManager.Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _mockJwtTokenGenerator.Setup(g => g.GenerateToken("user-id", "test@flira.com", "test_user", It.IsAny<IList<string>>()))
            .Returns("fake-jwt-token");

        var handler = new LoginQueryHandler(_mockUserManager.Object, _mockJwtTokenGenerator.Object, _context);
        var query = new LoginQuery("test@flira.com", "Password123!");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("fake-jwt-token", result.Value.Token);
        Assert.Equal("test@flira.com", result.Value.Email);
        Assert.Equal("user-id", result.Value.UserId);
        Assert.NotEmpty(result.Value.RefreshToken);

        var savedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == "user-id");
        Assert.NotNull(savedRefreshToken);
        Assert.Equal(result.Value.RefreshToken, savedRefreshToken.Token);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((IdentityUser)null);

        var handler = new LoginQueryHandler(_mockUserManager.Object, _mockJwtTokenGenerator.Object, _context);
        var query = new LoginQuery("unknown@flira.com", "Password123!");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidCredentials", result.Error.Code);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new IdentityUser { Id = "user-id", Email = "test@flira.com" };
        _mockUserManager.Setup(m => m.FindByEmailAsync("test@flira.com"))
            .ReturnsAsync(user);

        _mockUserManager.Setup(m => m.CheckPasswordAsync(user, "WrongPassword"))
            .ReturnsAsync(false);

        var handler = new LoginQueryHandler(_mockUserManager.Object, _mockJwtTokenGenerator.Object, _context);
        var query = new LoginQuery("test@flira.com", "WrongPassword");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidCredentials", result.Error.Code);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
