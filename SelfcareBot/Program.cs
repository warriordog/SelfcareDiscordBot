using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SelfcareBot.Config;
using SelfcareBot.DataLayer.context;
using SelfcareBot.Main;
using SelfcareBot.Services;

namespace SelfcareBot
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // Create environment
            using var host = CreateHost(args);
            
            // Start host
            await host.StartAsync();

            // Wait for graceful shutdown
            await host.WaitForShutdownAsync();
        }
        
        private static IHost CreateHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    // Inject config
                    services.AddOptions<HydrationOptions>()
                        .Bind(ctx.Configuration.GetSection(key: nameof(HydrationOptions)))
                        .ValidateDataAnnotations();
                    services.AddOptions<BotOptions>()
                        .Bind(ctx.Configuration.GetSection(key: nameof(BotOptions)))
                        .ValidateDataAnnotations();
                    services.AddOptions<SelfcareDatabaseOptions>()
                        .Bind(ctx.Configuration.GetSection(key: nameof(SelfcareDatabaseOptions)))
                        .ValidateDataAnnotations();
                    
                    // Inject database
                    services.AddDbContext<SelfcareDbContext>();
                    services.AddScoped<ISelfcareDbContext>(provider => provider.GetService<SelfcareDbContext>() ?? throw new InvalidOperationException("Required service SelfcareContext is not registered for DI."));
                    
                    // Inject services
                    services.AddSingleton<IHydrationLeaderboard, HydrationLeaderboard>();
                    
                    // Inject main app logic
                    services.AddScoped<SelfcareBotMain>();
                    services.AddHostedService<SelfcareBotService>();
                })
                .Build();
        }
    }
}