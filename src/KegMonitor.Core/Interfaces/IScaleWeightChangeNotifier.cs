namespace KegMonitor.Core.Interfaces
{
    public interface IScaleWeightChangeNotifier
    {
        Task NotifyAsync(int scaleId, int weight);
    }
}
