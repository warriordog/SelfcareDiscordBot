using System.Diagnostics.CodeAnalysis;
using DSharpPlus;

namespace SelfcareBot.Config
{
    public class DiscordConnectionOptions
    {
        [AllowNull]
        public string Token { get; set; }
    }
}