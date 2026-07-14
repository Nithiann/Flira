using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
