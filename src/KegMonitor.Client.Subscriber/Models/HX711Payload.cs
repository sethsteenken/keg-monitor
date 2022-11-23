namespace KegMonitor.Client.Subscriber
{
    internal class HX711Payload
    {
        public int Weight { get; set; }
        public int WeightRaw { get; set; }
        public int AbsRaw { get; set; }
    }
}
