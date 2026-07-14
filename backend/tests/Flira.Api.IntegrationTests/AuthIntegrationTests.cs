using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Flira.Application.Features.Auth;
using Flira.Application.Features.Auth.Commands.Register;
using Flira.Application.Features.Auth.Queries.Login;
using Xunit;

namespace Flira.Api.IntegrationTests;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterAndLogin_Should_RegisterUser_And_ReturnJwtAndRefreshTokensOnLogin()
    {
        // Arrange
        var uniqueEmail = $"user_{Guid.NewGuid():N}@flira.com";
        var registerCommand = new RegisterCommand(
            uniqueEmail,
            "Password123!",
            "Integratie Test"
        );

        // Act - Register
        var registerResponse = await _client.PostAsJsonAsync("api/auth/register", registerCommand);

        // Assert - Register
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterResponseDto>();
        Assert.NotNull(registerResult);
        Assert.NotEmpty(registerResult.UserId);

        // Arrange - Login
        var loginQuery = new LoginQuery(
            uniqueEmail,
            "Password123!"
        );

        // Act - Login
        var loginResponse = await _client.PostAsJsonAsync("api/auth/login", loginQuery);

        // Assert - Login
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginResult);
        Assert.Equal(uniqueEmail, loginResult.Email);
        Assert.Equal(registerResult.UserId, loginResult.UserId);
        Assert.NotEmpty(loginResult.Token);
        Assert.NotEmpty(loginResult.RefreshToken);
    }

    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_CredentialsAreInvalid()
    {
        // Arrange
        var loginQuery = new LoginQuery(
            "nonexistent-user-email@flira.com",
            "Password123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("api/auth/login", loginQuery);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private class RegisterResponseDto
    {
        public string UserId { get; set; } = string.Empty;
    }
}
