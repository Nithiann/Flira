using System;
using System.Threading.Tasks;

namespace Flira.Application.Security;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string userId, Guid organizationId, string permission);
}
