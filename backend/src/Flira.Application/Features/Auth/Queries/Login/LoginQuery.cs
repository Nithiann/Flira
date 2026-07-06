using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Auth.Queries.Login;

public record LoginQuery(string Email, string Password) : IRequest<Result<AuthResponse>>;
