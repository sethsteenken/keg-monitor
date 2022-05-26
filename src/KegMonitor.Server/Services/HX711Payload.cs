namespace KegMonitor.Server
{
    internal class HX711Payload
    {
        public long Weight { get; set; }
        public long WeightRaw { get; set; }
        public long AbsRaw { get; set; }
    }
}
