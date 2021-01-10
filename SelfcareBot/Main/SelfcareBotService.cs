using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SelfcareBot.Main
{
    public class SelfcareBotService : IHostedService
    {
        private readonly SelfcareBotMain _selfcareBotMain;

        public SelfcareBotService(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            _selfcareBotMain = scope.ServiceProvider.GetRequiredService<SelfcareBotMain>();
        }

        public Task StartAsync(CancellationToken _) => _selfcareBotMain.RunAsync();
        public Task StopAsync(CancellationToken _) => _selfcareBotMain.StopAsync();
    }
}