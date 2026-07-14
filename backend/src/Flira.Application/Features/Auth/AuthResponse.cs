namespace Flira.Application.Features.Auth;

public record AuthResponse(string Token, string RefreshToken, string Email, string UserId);
