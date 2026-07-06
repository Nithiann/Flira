using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Flira.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly UserManager<IdentityUser> _userManager;

    public ForgotPasswordCommandHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // Return success to prevent user enumeration attacks
            return Result.Success();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        // Log to console (in development) as an e-mail stub
        Console.WriteLine($"[EMAIL STUB] Password reset token for {user.Email}: {token}");

        return Result.Success();
    }
}
