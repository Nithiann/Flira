using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Flira.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly UserManager<IdentityUser> _userManager;

    public ResetPasswordCommandHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure(new Error("Auth.UserNotFound", "Gebruiker niet gevonden."));
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Wachtwoord herstellen mislukt.";
            return Result.Failure(new Error("Auth.ResetPasswordFailed", firstError));
        }

        return Result.Success();
    }
}
