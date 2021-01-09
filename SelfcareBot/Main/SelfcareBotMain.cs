using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace SelfcareBot.Main
{
    public class SelfcareBotMain
    {
        private readonly ServiceProvider _serviceProvider;
        
        public SelfcareBotMain(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task RunBot()
        {
            // Create discord client
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "Nzk3Mjc2MzE5OTIxODY0NzA0.X_kHbw.zlMRatfFYPnX05CylmQGDlmqto4",
                TokenType = TokenType.Bot       
            });
            
            // Enable interactivity
            discord.UseInteractivity(new InteractivityConfiguration() 
            { 
                PollBehaviour = PollBehaviour.KeepEmojis
            });
            
            // Register commands
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            { 
                StringPrefixes = new[] { "care*" },
                Services = _serviceProvider
            });
            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            // Connect to discord server
            await discord.ConnectAsync();
            
            // Run forever to prevent exit
            await Task.Delay(-1);
        }
    }
}