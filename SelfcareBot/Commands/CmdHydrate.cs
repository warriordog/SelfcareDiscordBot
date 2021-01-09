using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using SelfcareBot.Services;

namespace SelfcareBot.Commands
{
    public class CmdHydrate : BaseCommandModule
    {
        private readonly IHydrationLeaderboard _hydrationLeaderboard;

        public CmdHydrate(IHydrationLeaderboard hydrationLeaderboard)
        {
            _hydrationLeaderboard = hydrationLeaderboard;
        }

        [Command("hydrate")]
        [Description("Issues a call for hydration")]
        [RequireGuild]
        public async Task HydrateCommand(CommandContext ctx)
        {
            // Send hydration message
            var hydrateMessage = await ctx.RespondAsync("Its time to hydrate! Drink some water and then click the reaction below.");
            
            // Attach water emoji
            var waterEmoji = DiscordEmoji.FromName(ctx.Client, ":droplet:");
            await hydrateMessage.CreateReactionAsync(waterEmoji);

            // Wait for responses
            var responseTime = new TimeSpan(0, 0, 5);
            var usersWhoResponded = (await hydrateMessage.CollectReactionsAsync(responseTime))
                .Where(reaction => reaction.Emoji.Equals(waterEmoji))
                .SelectMany(reaction => reaction.Users)
                .Where(user => !user.Equals(ctx.Client.CurrentUser));

            // Remove message
            await hydrateMessage.DeleteAsync();
            
            // Award hydration points
            foreach (var user in usersWhoResponded)
            {
                await _hydrationLeaderboard.AwardPoints(user.Id);   
            }
        }

        [Command("scores")]
        [Description("Displays the current self-care leaderboard")]
        [RequireGuild]
        public async Task CareScoresCommand(CommandContext ctx)
        {
            // Setup leaderboard message
            var leaderboardLines = new List<string>()
            {
                "Hydration leaderboard:"   
            };
            
            // Append leaderboard rows to message
            var leaderboard = (await _hydrationLeaderboard.GetLeaderboard()).ToArray();
            if (leaderboard.Any())
            {
                foreach (var entry in leaderboard)
                {
                    var user = await ctx.Client.GetUserAsync(entry.UserId);
                    var userMention = DSharpPlus.Formatter.Mention(user, true);
                    var line = $"{entry.Rank.ToString()}. {userMention}: {entry.Score.ToString()}";
                    leaderboardLines.Add(line);
                }
            }
            else
            {
                leaderboardLines.Add("No results! Use `care*hydrate` to call for hydration.");
            }

            // Send message to channel
            var leaderboardMessage = string.Join("\n", leaderboardLines);
            await ctx.RespondAsync(leaderboardMessage);
        }
    }
}