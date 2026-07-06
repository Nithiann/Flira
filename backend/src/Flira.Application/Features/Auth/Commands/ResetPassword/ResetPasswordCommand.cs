using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result>;
