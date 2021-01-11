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
    [ModuleLifespan(ModuleLifespan.Transient)]
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
                var leaderboard = await _hydrationLeaderboard.GetLeaderboard(_hydrationOptions.LeaderboardSize);
                _logger.LogDebug("Got leaderboard");
                if (leaderboard.Any())
                {
                    var maxRankDecimals = (int) Math.Floor(Math.Log10(leaderboard.Count)) + 1;
                    var maxScoreDecimals = (int) Math.Floor(Math.Log10(leaderboard[0].Score)) + 1;
                    leaderboardLines.AddRange(leaderboard.Select(entry =>
                        $"#{entry.Rank.ToString().PadLeft(maxRankDecimals)}:\t{entry.Score.ToString().PadLeft(maxScoreDecimals)}\t{entry.Username}#{entry.Discriminator}")
                    );
                }
                else
                {
                    leaderboardLines.Add($"No results! Use the hydrate command to call for hydration.");
                }

                // Convert to inline code style
                var leaderboardText = Formatter.BlockCode(string.Join("\n", leaderboardLines));
                
                // Debug log leaderboard
                _logger.LogDebug("leaderboard entries: [{leaderboard}]", leaderboard);

                // Send message to channel
                var waterEmoji = DiscordEmoji.FromName(ctx.Client, _hydrationOptions.WaterEmojiName);
                var leaderboardMessage = $"{Formatter.Underline($"{waterEmoji}Hydration Leaderboard{waterEmoji}")}\n{leaderboardText}";
                await ctx.RespondAsync(leaderboardMessage);
            }
        }
    }
}