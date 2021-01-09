using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SelfcareBot.Config;

namespace SelfcareBot.Main
{
    public class SelfcareBotMain : IHostedService
    {
        private readonly DiscordClient _discord;
        
        public SelfcareBotMain(IServiceProvider serviceProvider, IOptions<BotLoggingOptions> botLoggingOptions)
        {
            // Create discord client
            _discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "Nzk3Mjc2MzE5OTIxODY0NzA0.X_kHbw.zlMRatfFYPnX05CylmQGDlmqto4",
                TokenType = TokenType.Bot,
                MinimumLogLevel = botLoggingOptions.Value.LogLevel
            });
            
            // Enable interactivity
            _discord.UseInteractivity(new InteractivityConfiguration() 
            { 
                PollBehaviour = PollBehaviour.KeepEmojis
            });
            
            // Register commands
            _discord.UseCommandsNext(new CommandsNextConfiguration()
            { 
                StringPrefixes = new[] { "care*" },
                Services = serviceProvider
            })
            .RegisterCommands(Assembly.GetExecutingAssembly());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Connect to discord server
            return _discord.ConnectAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _discord.DisconnectAsync();
        }
    }
}