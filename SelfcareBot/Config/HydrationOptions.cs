using System;

namespace SelfcareBot.Config
{
    public class HydrationOptions
    {
        public int LeaderboardSize { get; set; } = 3;
        
        public string WaterEmojiName { get; set; } = ":droplet:";

        public TimeSpan HydrateRequestExpiresAfter { get; set; } = new(0, 5, 0);

        public bool DeleteExpiredHydrationRequests { get; set; } = true;
    }
}