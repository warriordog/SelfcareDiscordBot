using System;
using System.ComponentModel.DataAnnotations;

namespace SelfcareBot.Config
{
    public class HydrationOptions
    {
        [Required] [Range(1, int.MaxValue)] public int LeaderboardSize { get; init; } = 3;

        [Required]
        [RegularExpression(@"^:[\w\d]+:$")]
        public string WaterEmojiName { get; init; } = ":droplet:";

        [Required] public TimeSpan HydrateRequestExpiresAfter { get; init; } = new(0, 5, 0);

        [Required] public bool DeleteExpiredHydrationRequests { get; init; } = true;
    }
}