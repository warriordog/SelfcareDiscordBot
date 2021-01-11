using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SelfcareBot.Config;
using SelfcareBot.DataLayer.context;
using SelfcareBot.Main;
using SelfcareBot.Services;

namespace SelfcareBot
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            // Create environment
            using var host = CreateHost(args);

            // Run application
            await host.RunAsync();
        }

        private static IHost CreateHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (ctx, services) =>
                    {
                        // Inject config
                        services.AddOptions<HydrationOptions>()
                            .Bind(ctx.Configuration.GetSection(nameof(HydrationOptions)))
                            .ValidateDataAnnotations();

                        services.AddOptions<BotOptions>()
                            .Bind(ctx.Configuration.GetSection(nameof(BotOptions)))
                            .ValidateDataAnnotations();

                        services.AddOptions<SelfcareDatabaseOptions>()
                            .Bind(ctx.Configuration.GetSection(nameof(SelfcareDatabaseOptions)))
                            .ValidateDataAnnotations();

                        // Inject database
                        services.AddDbContext<SelfcareDbContext>();
                        services.AddScoped<ISelfcareDbContext>(provider => provider.GetRequiredService<SelfcareDbContext>());

                        // Inject services
                        services.AddScoped<IUserService, UserService>();
                        services.AddScoped<IHydrationLeaderboard, HydrationLeaderboard>();

                        // Inject main app logic
                        services.AddScoped<SelfcareBotMain>();
                        services.AddHostedService<SelfcareBotService>();
                    }
                )
                .Build();
        }
    }
}