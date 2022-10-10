using MudBlazor;

namespace KegMonitor.Web.Application
{
    public class ScaleChart
    {
        public ChartSeries Data { get; set; }
        public string[] XLabels { get; set; }
        public ChartOptions Options { get; set; } = new ChartOptions()
        {
            XAxisLines = true,
            YAxisLines = true,
            YAxisTicks = 1,
            YAxisFormat = String.Empty,
            DisableLegend = true
        };
    }
}
