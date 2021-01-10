using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SelfcareBot.Config;
using SelfcareBot.Services;

namespace SelfcareBot.Commands
{
    public class CmdScores : BaseCommandModule
    {
        private readonly IHydrationLeaderboard _hydrationLeaderboard;
        private readonly ILogger<CmdScores> _logger;
        private readonly HydrationOptions _hydrationOptions;

        public CmdScores(IHydrationLeaderboard hydrationLeaderboard, ILogger<CmdScores> logger, IOptions<HydrationOptions> hydrationOptions)
        {
            _hydrationLeaderboard = hydrationLeaderboard;
            _logger = logger;
            _hydrationOptions = hydrationOptions.Value;
        }
        
        [Command("scores")]
        [Description("Displays the current self-care leaderboard")]
        [RequireGuild]
        public async Task ScoresCommand(CommandContext ctx)
        {
            // Setup logging context
            using (_logger.BeginScope($"CmdScores.ScoresCommand@{ctx.Message.Id.ToString()}"))
            {
                _logger.LogDebug("Requested by [{user}]", ctx.User);
                
                // Create leaderboard text
                var leaderboardLines = new List<string>();
                var leaderboard = (await _hydrationLeaderboard.GetLeaderboard(_hydrationOptions.LeaderboardSize)).ToArray();
                if (leaderboard.Any())
                {
                    var maxRankDecimals = (int) Math.Floor(Math.Log10(leaderboard.Length)) + 1;
                    var maxScoreDecimals = (int) Math.Floor(Math.Log10(leaderboard[0].Score)) + 1;
                    foreach (var entry in leaderboard)
                    {
                        var memberName = await GetUserDisplayName(entry.UserId, ctx.Client, ctx.Guild);
                        var line = $"#{entry.Rank.ToString().PadLeft(maxRankDecimals)}:\t{entry.Score.ToString().PadLeft(maxScoreDecimals)}\t{memberName}";
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
                var waterEmoji = DiscordEmoji.FromName(ctx.Client, _hydrationOptions.WaterEmojiName);
                var leaderboardMessage = $"{Formatter.Underline($"{waterEmoji}Hydration Leaderboard{waterEmoji}")}\n{leaderboardText}";
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