using Microsoft.Extensions.Logging;

namespace KegMonitor.Core
{
    public static class LoggerExtensions
    {
        public static void Log(this ILogger logger, LogMessage logMessage)
        {
            if (!Enum.TryParse<LogLevel>(logMessage.Level, out LogLevel level))
                level = LogLevel.Warning;

            logger.Log(level, logMessage.Message);
        }
    }
}
