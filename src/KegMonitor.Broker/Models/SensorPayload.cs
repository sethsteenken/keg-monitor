namespace KegMonitor.Broker
{
    internal class SensorPayload
    {
        public DateTime Time { get; set; }
        public HX711Payload HX711 { get; set; } = new HX711Payload();
    }
}
