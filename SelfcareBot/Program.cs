using Microsoft.Extensions.DependencyInjection;
using SelfcareBot.Main;
using SelfcareBot.Services;

namespace SelfcareBot
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Init services
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IHydrationLeaderboard, HydrationLeaderboard>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Run bot
            var main = new SelfcareBotMain(serviceProvider);
            main.RunBot().GetAwaiter().GetResult();
        }
    }
}