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

        public HydrationLeaderboard(ISelfcareDbContext selfcareDb)
        {
            _selfcareDb = selfcareDb;
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
            // Start DB transaction
            await using var transaction = await _selfcareDb.BeginTransactionAsync();
            
            // Get user (if present)
            var knownUser = await _selfcareDb.KnownUsers
                .Where(kn => kn.DiscordId == discordUser.Id)
                .FirstOrDefaultAsync();
            
            // Record user if not present
            if (knownUser == null)
            {
                knownUser = new KnownUser()
                {
                    DiscordId = discordUser.Id,
                    Username = discordUser.Username,
                    Discriminator = discordUser.Discriminator
                };
                _selfcareDb.KnownUsers.Add(knownUser);
            }
            
            // Get current score (if present)
            var userScore = await _selfcareDb.UserScores
                .Where(us => us.KnownUser.Id == knownUser.Id)
                .FirstOrDefaultAsync();

            // If user already has a score, then update
            if (userScore != null)
            {
                userScore.Score++;
                _selfcareDb.UserScores.Update(userScore);
            }
            // If user does not have a score, then insert
            else
            {
                userScore = new UserScore()
                {
                    KnownUser = knownUser,
                    Category = HydrationCategory,
                    Score = points
                };
                _selfcareDb.UserScores.Add(userScore);
            }

            // Save changes
            await transaction.CommitAsync();
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