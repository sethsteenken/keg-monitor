namespace KegMonitor.Web.Application
{
    public interface IScaleDisplayQueryService
    {
        Task<List<ScaleDisplayItem>> GetScalesAsync(bool activeOnly = false);
    }
}
