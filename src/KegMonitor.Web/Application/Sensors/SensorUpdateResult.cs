namespace KegMonitor.Web.Application
{
    public class SensorUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static SensorUpdateResult Failed(string message) => new SensorUpdateResult { Success = false, Message = message };
        public static SensorUpdateResult Succeeded(string message) => new SensorUpdateResult { Success = true, Message = message };
    }
}
