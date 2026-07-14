using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Users.Queries.SearchUsers;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, Result<List<UserSearchResultDto>>>
{
    private readonly UserManager<IdentityUser> _userManager;

    public SearchUsersQueryHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<List<UserSearchResultDto>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return Result.Success(new List<UserSearchResultDto>());
        }

        var normalizedSearchTerm = request.SearchTerm.ToUpperInvariant();

        var users = await _userManager.Users
            .Where(u => u.Id != request.CurrentUserId &&
                        ((u.NormalizedUserName != null && u.NormalizedUserName.Contains(normalizedSearchTerm)) || 
                         (u.NormalizedEmail != null && u.NormalizedEmail.Contains(normalizedSearchTerm))))
            .Take(10)
            .Select(u => new UserSearchResultDto(u.Id, u.UserName ?? "", u.Email ?? ""))
            .ToListAsync(cancellationToken);

        return Result.Success(users);
    }
}
