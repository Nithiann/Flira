using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Flira.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterCommandHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result.Failure<string>(new Error("Auth.DuplicateEmail", "Dit e-mailadres is al in gebruik."));
        }

        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Registratie mislukt.";
            return Result.Failure<string>(new Error("Auth.RegistrationFailed", firstError));
        }

        // Add default Role "User"
        await _userManager.AddToRoleAsync(user, "User");

        return Result.Success(user.Id);
    }
}
