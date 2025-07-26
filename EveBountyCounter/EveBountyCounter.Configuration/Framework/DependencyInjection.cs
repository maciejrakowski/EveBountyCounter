using Microsoft.Extensions.DependencyInjection;

namespace EveBountyHunter.Configuration.Framework;

public static class DependencyInjection
{
    public static IServiceCollection UseEbhConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IEbhConfiguration, EbhConfiguration>();
        
        return services;
    }
}