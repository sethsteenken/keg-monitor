﻿namespace KegMonitor.Web
{
    public class MqttBrokerSettings
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string ClientId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Topics { get; set; } = new List<string>();
    }
}
