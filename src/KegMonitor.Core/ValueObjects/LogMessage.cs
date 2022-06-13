namespace KegMonitor.Core
{
    public class LogMessage
    {
        public DateTime? Timestamp { get; set; }
        public string? Logger { get; set; }
        public string? Level { get; set; }
        public string? Message { get; set; }
    }
}
