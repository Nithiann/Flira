using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Users.Queries.GetUserProfile;

public record GetUserProfileQuery(string UserId) : IRequest<Result<UserProfileDto>>;

public record UserProfileDto(
    string UserId,
    string Username,
    string Email,
    string GlobalRole,
    List<UserOrganizationDto> Organizations,
    List<UserProjectDto> Projects);

public record UserOrganizationDto(
    Guid Id,
    string Name,
    string Role);

public record UserProjectDto(
    Guid Id,
    string Name,
    string Color,
    string Icon,
    string OrganizationName);
