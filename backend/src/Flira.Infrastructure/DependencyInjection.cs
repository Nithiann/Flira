using Flira.Application.Interfaces;
using Flira.Application.Security;
using Flira.Infrastructure.Authentication;
using Flira.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Flira.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPermissionService, PermissionService>();
        
        return services;
    }
}
