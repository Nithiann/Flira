using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Flira.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Flira.Api.Security;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IPermissionService permissionService)
    {
        _httpContextAccessor = httpContextAccessor;
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        if (!httpContext.Request.Headers.TryGetValue("X-Organization-Id", out var orgIdHeader) ||
            !Guid.TryParse(orgIdHeader.ToString(), out var organizationId))
        {
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                     context.User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var hasPermission = await _permissionService.HasPermissionAsync(userId, organizationId, requirement.Permission);
        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
