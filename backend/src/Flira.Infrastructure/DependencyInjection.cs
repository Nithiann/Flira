using Flira.Application.Interfaces;
using Flira.Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Flira.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        
        return services;
    }
}
