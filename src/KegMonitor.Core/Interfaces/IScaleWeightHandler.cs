namespace KegMonitor.Core.Interfaces
{
    public interface IScaleWeightHandler
    {
        Task HandleAsync(int scaleId, int weight);
    }
}
