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
        private readonly ILogger<SelfcareBotMain> _logger;
        
        public SelfcareBotMain(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, ILogger<SelfcareBotMain> logger, IOptions<BotOptions> botOptions)
        {
            _logger = logger;
            
            // Log initialization
            using (_logger.BeginScope("Configuration"))
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
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (_logger.BeginScope("Startup"))
            {
                await _discord.ConnectAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using (_logger.BeginScope("Shutdown"))
            {
                await _discord.DisconnectAsync();
            }
        }
    }
}