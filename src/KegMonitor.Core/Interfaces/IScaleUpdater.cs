namespace KegMonitor.Core.Interfaces
{
    public interface IScaleUpdater
    {
        Task<ScaleUpdateResult> UpdateAsync(int scaleId, int weight);
    }
}
