using MudBlazor;

namespace KegMonitor.Web.Application
{
    public class ScaleDashboardModel
    {
        public IEnumerable<ScaleDisplayItem> Scales { get; set; } = new List<ScaleDisplayItem>();
        public List<ChartSeries> ChartSeries { get; set; } = new List<ChartSeries>();
        public string[] ChartXLabels { get; set; }
    }
}
