using System;
using System.ComponentModel.DataAnnotations;

namespace SelfcareBot.Config
{
    public class HydrationOptions
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int LeaderboardSize { get; set; } = 3;
        
        [Required]
        [RegularExpression(@"^:[\w\d]+:$")]
        public string WaterEmojiName { get; set; } = ":droplet:";

        [Required]
        public TimeSpan HydrateRequestExpiresAfter { get; set; } = new(0, 5, 0);

        [Required]
        public bool DeleteExpiredHydrationRequests { get; set; } = true;
    }
}