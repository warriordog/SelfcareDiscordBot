using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SelfcareBot.Config;
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
                    services.Configure<DiscordConnectionOptions>(ctx.Configuration.GetSection(key: nameof(DiscordConnectionOptions)));
                    
                    // Inject services
                    services
                        .AddSingleton<IHydrationLeaderboard, HydrationLeaderboard>()
                        .AddHostedService<SelfcareBotMain>()
                    ;
                })
                .Build();
        }
    }
}