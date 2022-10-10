using MudBlazor;

namespace KegMonitor.Web.Application
{
    public class ScaleDashboardModel
    {
        public IEnumerable<ScaleDashboardItem> Scales { get; set; } = new List<ScaleDashboardItem>();
    }
}
