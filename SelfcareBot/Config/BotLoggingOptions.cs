using Microsoft.Extensions.Logging;

namespace SelfcareBot.Config
{
    public class BotLoggingOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
    }
}