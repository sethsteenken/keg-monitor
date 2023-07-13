﻿namespace KegMonitor.Web
{
    public class MqttBrokerSettings
    {
        public bool Subscribe { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string ClientId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
