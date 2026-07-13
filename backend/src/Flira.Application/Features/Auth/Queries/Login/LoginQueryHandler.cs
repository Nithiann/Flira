using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Flira.Application.Features.Auth.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<AuthResponse>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IApplicationDbContext _context;

    public LoginQueryHandler(
        UserManager<IdentityUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _context = context;
    }

    public async Task<Result<AuthResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.InvalidCredentials", "Ongeldige inloggegevens."));
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.InvalidCredentials", "Ongeldige inloggegevens."));
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, user.UserName ?? user.Email!, roles);

        // Generate and save Refresh Token
        var refreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResponse(token, refreshToken.Token, user.Email!, user.Id));
    }
}
