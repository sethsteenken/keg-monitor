namespace KegMonitor.Web.Application
{
    public interface IScaleCommandService
    {
        Task<int> AddAsync(ScaleAddModel model);
        Task UpdateActiveStateAsync(int scaleId, bool active);
        Task<int> SaveAsync(ScaleEditModel model);
        Task DeleteAsync(int id);
        Task PurgeAllWeightMetricsAsync();
    }
}
