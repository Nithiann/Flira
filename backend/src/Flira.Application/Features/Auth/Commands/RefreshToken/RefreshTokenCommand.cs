using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string Token) : IRequest<Result<AuthResponse>>;
