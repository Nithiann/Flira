using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string FullName) : IRequest<Result<string>>;
