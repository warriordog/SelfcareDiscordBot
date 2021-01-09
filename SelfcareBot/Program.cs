using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                .ConfigureServices((_, services) => services
                    .AddSingleton<IHydrationLeaderboard, HydrationLeaderboard>()
                    .AddHostedService<SelfcareBotMain>()
                )
                .Build();
        }
    }
}