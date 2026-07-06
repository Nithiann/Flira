using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Application.Security;
using Flira.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace Flira.Infrastructure.Security;

public class PermissionService : IPermissionService
{
    private readonly IApplicationDbContext _context;

    private static readonly Dictionary<string, HashSet<string>> RolePermissions = new()
    {
        {
            "Admin", new HashSet<string>
            {
                Permissions.ProjectCreate, Permissions.ProjectRead, Permissions.ProjectUpdate, Permissions.ProjectDelete,
                Permissions.TaskCreate, Permissions.TaskRead, Permissions.TaskUpdate, Permissions.TaskDelete,
                Permissions.TeamManage, Permissions.OrganizationManage
            }
        },
        {
            "Manager", new HashSet<string>
            {
                Permissions.ProjectCreate, Permissions.ProjectRead, Permissions.ProjectUpdate,
                Permissions.TaskCreate, Permissions.TaskRead, Permissions.TaskUpdate, Permissions.TaskDelete,
                Permissions.TeamManage
            }
        },
        {
            "Member", new HashSet<string>
            {
                Permissions.ProjectRead,
                Permissions.TaskCreate, Permissions.TaskRead, Permissions.TaskUpdate
            }
        },
        {
            "Guest", new HashSet<string>
            {
                Permissions.ProjectRead, Permissions.TaskRead
            }
        }
    };

    public PermissionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(string userId, Guid organizationId, string permission)
    {
        var membership = await _context.OrganizationUsers
            .FirstOrDefaultAsync(ou => ou.UserId == userId && ou.OrganizationId == organizationId);

        if (membership == null)
        {
            return false;
        }

        if (RolePermissions.TryGetValue(membership.Role, out var permissions))
        {
            return permissions.Contains(permission);
        }

        return false;
    }
}
