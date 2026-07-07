using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Flira.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);
        
        // Note: AutoMapper is registered here if profiles exist, but we scan the assembly.
        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        return services;
    }
}
