using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfcareBot.Services
{
    public interface IHydrationLeaderboard
    {
        public Task<IEnumerable<HydrationLeaderboardEntry>> GetLeaderboard(int top = 3);
        public Task AwardPoints(ulong userId, int points = 1);
    }

    public class HydrationLeaderboard : IHydrationLeaderboard
    {
        private readonly IDictionary<ulong, int> _hydrationScores = new Dictionary<ulong, int>();

        public Task<IEnumerable<HydrationLeaderboardEntry>> GetLeaderboard(int top = 3)
        {
            var leaderboard = _hydrationScores
                .OrderByDescending(entry => entry.Value)
                .Take(top)
                .Select((entry, idx) => new HydrationLeaderboardEntry
                {
                    UserId = entry.Key,
                    Score = entry.Value,
                    Rank = idx + 1
                });
            
            return Task.FromResult(leaderboard);
        }

        public Task AwardPoints(ulong userId, int points = 1)
        {
            if (_hydrationScores.TryGetValue(userId, out var previousPoints))
            {
                _hydrationScores[userId] = previousPoints + points;
            }
            else
            {
                _hydrationScores[userId] = points;
            }

            return Task.CompletedTask;
        }
    }

    public class HydrationLeaderboardEntry
    {
        public ulong UserId { get; init; }
        
        public int Score { get; init; }
        
        public int Rank { get; init; }

        public override string ToString() => $"{{\"UserId\"=\"{UserId.ToString()}\", \"Score\"=\"{Score.ToString()}\", \"Rank\"=\"{Rank.ToString()}\"}}";
    }
}