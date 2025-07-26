using Microsoft.Extensions.DependencyInjection;

namespace EveBountyCounter.EwbApiClient.Framework;

public static class DependencyInjection
{
    public static IServiceCollection UseEwbApiClient(this IServiceCollection services)
    {
        services.AddHttpClient();
        
        services.AddScoped<IEwbApiClient, EwbApiClient>(); 
        
        return services;
    }
}