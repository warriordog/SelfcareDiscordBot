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
        
        public SelfcareBotMain(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<BotOptions> botOptions)
        {
            // Create discord client
            _discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = botOptions.Value.DiscordToken,
                TokenType = TokenType.Bot,
                LoggerFactory = loggerFactory
            });
        
            // Enable interactivity
            _discord.UseInteractivity(new InteractivityConfiguration() 
            { 
                PollBehaviour = PollBehaviour.KeepEmojis
            });
        
            // Register commands
            _discord.UseCommandsNext(new CommandsNextConfiguration()
            { 
                StringPrefixes = botOptions.Value.CommandPrefixes,
                Services = serviceProvider
            })
            .RegisterCommands(Assembly.GetExecutingAssembly());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _discord.ConnectAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _discord.DisconnectAsync();
        }
    }
}