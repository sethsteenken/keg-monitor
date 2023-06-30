namespace KegMonitor.Core.Interfaces
{
    public interface IScaleWeightHandler
    {
        Task HandleAsync(string topic, int weight);
    }
}
