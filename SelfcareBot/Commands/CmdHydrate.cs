using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SelfcareBot.Config;
using SelfcareBot.Services;

namespace SelfcareBot.Commands
{
    public class CmdHydrate : BaseCommandModule
    {
        private readonly IHydrationLeaderboard _hydrationLeaderboard;
        private readonly ILogger<CmdHydrate> _logger;
        private readonly HydrationOptions _hydrationOptions;

        public CmdHydrate(IHydrationLeaderboard hydrationLeaderboard, ILogger<CmdHydrate> logger, IOptions<HydrationOptions> hydrationOptions)
        {
            _hydrationLeaderboard = hydrationLeaderboard;
            _logger = logger;
            _hydrationOptions = hydrationOptions.Value;
        }

        [Command("hydrate")]
        [Description("Issues a call for hydration")]
        [RequireGuild]
        public async Task HydrateCommand(CommandContext ctx)
        {
            // Setup logging context
            using (_logger.BeginScope($"CmdHydrate.HydrateCommand@{ctx.Message.Id.ToString()}"))
            {
                _logger.LogDebug("Requested by [{user}]", ctx.User);
                
                // Send hydration message
                var hydrateMessage = await ctx.RespondAsync("Its time to hydrate! Drink some water and then click the reaction below.");
            
                // Attach water emoji
                var waterEmoji = DiscordEmoji.FromName(ctx.Client, _hydrationOptions.WaterEmojiName);
                await hydrateMessage.CreateReactionAsync(waterEmoji);

                // Wait for responses
                var usersWhoResponded = (await hydrateMessage.CollectReactionsAsync(_hydrationOptions.HydrateRequestExpiresAfter))
                    .Where(reaction => reaction.Emoji.Equals(waterEmoji))
                    .SelectMany(reaction => reaction.Users)
                    .Where(user => !user.Equals(ctx.Client.CurrentUser))
                    .ToList();

                // Debug log responding users
                _logger.LogDebug("{count} users responded: [{users}]", usersWhoResponded.Count.ToString(), usersWhoResponded);
                
                // Remove message
                if (_hydrationOptions.DeleteExpiredHydrationRequests)
                {
                    await hydrateMessage.DeleteAsync();   
                }
            
                // Award hydration points
                foreach (var user in usersWhoResponded)
                {
                    await _hydrationLeaderboard.AwardPoints(user.Id);   
                }
            }
        }

        [Command("scores")]
        [Description("Displays the current self-care leaderboard")]
        [RequireGuild]
        public async Task ScoresCommand(CommandContext ctx)
        {
            // Setup logging context
            using (_logger.BeginScope($"CmdHydrate.ScoresCommand@{ctx.Message.Id.ToString()}"))
            {
                _logger.LogDebug("Requested by [{user}]", ctx.User);

                // Setup leaderboard message
                var leaderboardLines = new List<string>
                {
                    "Hydration leaderboard:"
                };

                // Append leaderboard rows to message
                var leaderboard = (await _hydrationLeaderboard.GetLeaderboard(_hydrationOptions.LeaderboardSize)).ToArray();
                if (leaderboard.Any())
                {
                    foreach (var entry in leaderboard)
                    {
                        var user = await ctx.Client.GetUserAsync(entry.UserId);
                        var userMention = Formatter.Mention(user, true);
                        var line = $"{entry.Rank.ToString()}. {userMention}: {entry.Score.ToString()}";
                        leaderboardLines.Add(line);
                    }
                }
                else
                {
                    leaderboardLines.Add($"No results! Use {Formatter.InlineCode("care*hydrate")} to call for hydration.");
                }

                // Debug log leaderboard
                _logger.LogDebug("leaderboard entries: [{leaderboard}]", new object[]{ leaderboard });

                // Send message to channel
                var leaderboardMessage = string.Join("\n", leaderboardLines);
                await ctx.RespondAsync(leaderboardMessage);
            }
        }
    }
}