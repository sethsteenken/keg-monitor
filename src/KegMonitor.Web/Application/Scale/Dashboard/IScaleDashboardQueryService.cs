namespace KegMonitor.Web.Application
{
    public interface IScaleDashboardQueryService
    {
        Task<List<ScaleDisplayItem>> GetScalesAsync();
        Task<List<int>> GetScaleIdsAsync();
        Task<ScaleDashboardModel> BuildModelAsync();
    }
}
