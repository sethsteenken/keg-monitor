using MudBlazor;

namespace KegMonitor.Web.Application
{
    public interface IScaleDashboardQueryService
    {
        Task<ScaleChart> GetScaleChartAsync(int scaleId, int numOfWeightChanges);
        Task<List<int>> GetScaleIdsAsync();
        Task<ScaleDashboardModel> BuildModelAsync(int numOfWeightChanges);
    }
}
