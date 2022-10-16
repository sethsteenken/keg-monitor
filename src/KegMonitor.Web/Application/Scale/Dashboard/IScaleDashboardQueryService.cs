using MudBlazor;

namespace KegMonitor.Web.Application
{
    public interface IScaleDashboardQueryService
    {
        Task<ScaleDashboardModel> BuildModelAsync();
        Task<ScaleWeightMetricsData> GetWeightMetricsAsync(int scaleId, int numOfWeightChanges);
        Task<List<int>> GetScaleIdsAsync();
       
    }
}
