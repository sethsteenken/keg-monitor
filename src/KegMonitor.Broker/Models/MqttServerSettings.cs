namespace KegMonitor.Broker
{
    public class MqttServerSettings
    {
        public string ServiceName { get; set; } = "MqttServer";
        public bool Enabled { get; set; }
        public IEnumerable<string>? AllowedClientIds { get; set; }
        public int HeartbeatDelay { get; set; } = 30000;
        public string? IpAddress { get; set; }
        public int Port { get; set; }
    }
}
