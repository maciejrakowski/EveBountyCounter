using EveBountyCounter;
using EveBountyCounter.EwbApiClient.Framework;
using EveBountyHunter.Configuration.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        // Register your services
        builder.Services.AddSingleton<IConfigurationSetup, ConfigurationSetup>();
        builder.Services.AddTransient<CounterWorker>();
        builder.Services.AddSingleton<ConsoleApplication>();

        builder.Services.UseEbhConfiguration();
        builder.Services.UseEwbApiClient();
        
        //Add logging
        builder.Services.AddLogging(logging => 
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Error);
        });
        
        var host = builder.Build();
        
        // Get the application service and run it
        var app = host.Services.GetRequiredService<ConsoleApplication>();
        await app.RunAsync();
    }
}