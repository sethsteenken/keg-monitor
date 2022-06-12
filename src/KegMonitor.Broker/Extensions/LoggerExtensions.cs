namespace KegMonitor.Broker
{
    public static class LoggerExtensions
    {
        public static void LogMemoryInformation(this ILogger logger, string serviceName, double bytesDivider = 1048576.0)
        {
            var totalMemory = GC.GetTotalMemory(false);
            var memoryInfo = GC.GetGCMemoryInfo();
            logger.LogInformation(
                "Heartbeat for service {ServiceName}: Total {Total}, heap size: {HeapSize}, memory load: {MemoryLoad}.",
                serviceName, $"{(totalMemory / bytesDivider):N3}", $"{(memoryInfo.HeapSizeBytes / bytesDivider):N3}", $"{(memoryInfo.MemoryLoadBytes / bytesDivider):N3}");
        }
    }
}
