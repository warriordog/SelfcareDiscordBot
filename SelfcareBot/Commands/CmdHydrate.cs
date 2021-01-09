using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

                // Create leaderboard text
                var leaderboardLines = new List<string>();
                var leaderboard = (await _hydrationLeaderboard.GetLeaderboard(_hydrationOptions.LeaderboardSize)).ToArray();
                if (leaderboard.Any())
                {
                    foreach (var entry in leaderboard)
                    {
                        var memberName = await GetUserDisplayName(entry.UserId, ctx.Client, ctx.Guild);
                        var line = $"{entry.Rank.ToString()}: {memberName} ({entry.Score.ToString()})";
                        leaderboardLines.Add(line);
                    }
                }
                else
                {
                    leaderboardLines.Add($"No results! Use the hydrate command to call for hydration.");
                }

                // Convert to inline code style
                var leaderboardText = Formatter.BlockCode(string.Join("\n", leaderboardLines));
                
                // Debug log leaderboard
                _logger.LogDebug("leaderboard entries: [{leaderboard}]", new object[]{ leaderboard });

                // Send message to channel
                var leaderboardMessage = $"Hydration leaderboard:\n{leaderboardText}";
                await ctx.RespondAsync(leaderboardMessage);
            }
        }

        private static async Task<string> GetUserDisplayName(ulong userId, DiscordClient client, [AllowNull] DiscordGuild guild = null)
        {
            // Get from guild, if possible
            if (guild != null)
            {
                var member = await guild.GetMemberAsync(userId);
                if (member != null)
                {
                    return $"{member.DisplayName}#{member.Discriminator}";
                }
            }
            
            // Otherwise fall back to global
            var user = await client.GetUserAsync(userId);
            return $"{user.Username}#{user.Discriminator}";
        }
    }
}