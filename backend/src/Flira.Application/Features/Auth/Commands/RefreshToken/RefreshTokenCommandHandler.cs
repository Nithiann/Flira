using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var activeToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.Token && t.RevokedAt == null, cancellationToken);

        if (activeToken == null || activeToken.IsExpired)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.InvalidRefreshToken", "Ongeldige of verlopen refresh token."));
        }

        var user = await _userManager.FindByIdAsync(activeToken.UserId);
        if (user == null)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.UserNotFound", "Gebruiker niet gevonden."));
        }

        // Revoke the old token
        activeToken.RevokedAt = DateTime.UtcNow;

        // Generate a new JWT
        var roles = await _userManager.GetRolesAsync(user);
        var jwtToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, user.UserName ?? user.Email!, roles);

        // Generate a new Refresh Token
        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResponse(jwtToken, newRefreshToken.Token, user.Email!, user.Id));
    }
}
