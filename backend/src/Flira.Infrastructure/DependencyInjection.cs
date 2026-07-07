using Flira.Application.Interfaces;
using Flira.Application.Security;
using Flira.Infrastructure.Authentication;
using Flira.Infrastructure.Security;
using Flira.Infrastructure.Services;
using Flira.Infrastructure.Services.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flira.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        var storageProvider = configuration["StorageSettings:Provider"] ?? "Local";
        if (storageProvider.Equals("Azure", System.StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IStorageService, AzureBlobStorageService>();
        }
        else
        {
            services.AddScoped<IStorageService, LocalStorageService>();
        }
        
        return services;
    }
}
