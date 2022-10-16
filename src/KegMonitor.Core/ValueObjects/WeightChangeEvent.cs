namespace KegMonitor.Core
{
    public record WeightChangeEvent(int Weight, DateTime TimeStamp, bool IsPourEvent);
}
