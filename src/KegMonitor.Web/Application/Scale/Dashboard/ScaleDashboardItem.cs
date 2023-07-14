namespace KegMonitor.Web.Application
{
    public class ScaleDashboardItem : ScaleDisplayItem
    {
        public ScaleSensor Sensor { get; set; } = new ScaleSensor();
        public ScaleWeightMetricsData WeightMetricsData { get; set; }
    }
}
