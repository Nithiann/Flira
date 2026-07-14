using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Users.Queries.SearchUsers;

public record SearchUsersQuery(string SearchTerm, string CurrentUserId) : IRequest<Result<List<UserSearchResultDto>>>;

public record UserSearchResultDto(string Id, string UserName, string Email);
