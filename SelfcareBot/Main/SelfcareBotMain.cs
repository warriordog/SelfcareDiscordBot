using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SelfcareBot.Commands;
using SelfcareBot.Config;
using SelfcareBot.DataLayer.context;
using SelfcareBot.Services;

namespace SelfcareBot.Main
{
    public class SelfcareBotMain
    {
        private readonly DiscordClient _discord;
        private readonly ISelfcareDbContext _selfcareDb;
        private readonly ILogger<SelfcareBotMain> _logger;
        
        public SelfcareBotMain(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<BotOptions> botOptions, ISelfcareDbContext selfcareDb, ILogger<SelfcareBotMain> logger)
        {
            _selfcareDb = selfcareDb;
            _logger = logger;

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

        public async Task StartAsync()
        {
            _logger.LogInformation($"SelfcareBot {GetType().Assembly.GetName().Version} starting");
            await _selfcareDb.MigrateAsync();
            await _discord.ConnectAsync();
        }

        public Task StopAsync()
        {
            _logger.LogInformation($"SelfcareBot stopping");
            return _discord.DisconnectAsync();
        }
    }
}