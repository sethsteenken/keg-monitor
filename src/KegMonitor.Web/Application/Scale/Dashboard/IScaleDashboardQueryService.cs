namespace KegMonitor.Web.Application
{
    public interface IScaleDashboardQueryService
    {
        Task<List<ScaleDisplayItem>> GetScalesAsync();
    }
}
