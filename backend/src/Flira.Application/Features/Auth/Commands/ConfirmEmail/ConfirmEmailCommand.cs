using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Auth.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string UserId, string Token) : IRequest<Result>;
