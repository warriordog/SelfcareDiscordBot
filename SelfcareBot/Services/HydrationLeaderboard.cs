using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using SelfcareBot.DataLayer.context;
using SelfcareBot.DataLayer.entities;

namespace SelfcareBot.Services
{
    public interface IHydrationLeaderboard
    {
        public Task<List<HydrationLeaderboardEntry>> GetLeaderboard(int top = 3);
        public Task AwardPoints(DiscordUser discordUser, int points = 1);
    }

    public class HydrationLeaderboard : IHydrationLeaderboard
    {
        private const string HydrationCategory = "hydration";
        
        private readonly ISelfcareDbContext _selfcareDb;
        private readonly IUserService _userService;

        public HydrationLeaderboard(ISelfcareDbContext selfcareDb, IUserService userService)
        {
            _selfcareDb = selfcareDb;
            _userService = userService;
        }

        public async Task<List<HydrationLeaderboardEntry>> GetLeaderboard(int top = 3)
        {
            var scores = await _selfcareDb.UserScores
                .Where(us => us.Category == HydrationCategory)
                .OrderByDescending(us => us.Score)
                .Take(top)
                .ToListAsync();
            
            return scores
                .Select((us, idx) => new HydrationLeaderboardEntry(
                    us.KnownUser.DiscordId,
                    us.KnownUser.Username,
                    us.KnownUser.Discriminator,
                    us.Score,
                    idx + 1
                ))
                .ToList();
        }

        public async Task AwardPoints(DiscordUser discordUser, int points = 1)
        {
            // Get user
            var knownUser = await _userService.GetOrCreateKnownUserForDiscordUser(discordUser);
            
            // Get current score (if present)
            var userScore = await _selfcareDb.UserScores
                .Where(us => us.KnownUser.Id == knownUser.Id)
                .FirstOrDefaultAsync();

            // If user does not have a score, then create it
            if (userScore == null)
            {
                userScore = new UserScore()
                {
                    KnownUser = knownUser,
                    Category = HydrationCategory,
                    Score = 0
                };
                _selfcareDb.UserScores.Add(userScore);
            }
        
            // give points
            userScore.Score += points;

            // Save changes
            await _selfcareDb.SaveChangesAsync();
        }
    }

    public class HydrationLeaderboardEntry
    {
        public ulong UserId { get; }
        
        public string Username { get; }
        
        public string Discriminator { get; }
        
        public int Score { get; }
        
        public int Rank { get; }

        public HydrationLeaderboardEntry(ulong userId, string username, string discriminator, int score, int rank)
        {
            UserId = userId;
            Username = username;
            Discriminator = discriminator;
            Score = score;
            Rank = rank;
        }

        public override string ToString() => $"[UserId={UserId}, Username='{Username}', Discriminator='{Discriminator}', Score={Score}, Rank={Rank}]";
    }
}