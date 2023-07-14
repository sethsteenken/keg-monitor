namespace KegMonitor.Web.Application
{
    public class ScaleDashboardItem : ScaleDisplayItem
    {
        public ScaleSensor Sensor { get; set; } = new ScaleSensor();
        public ScaleWeightMetricsData WeightMetricsData { get; set; }

        public async Task InitializeSensorAsync()
        {
            if (WeightMetricsData != null
                && WeightMetricsData.WeightChangesForDisplay.Any()
                && WeightMetricsData.WeightChangesForDisplay.Select(w => w.TimeStamp).First() >= DateTime.Now.AddSeconds(-15))
            {
                await Sensor.BringOnlineAsync();
            }
            else
                await Sensor.ResetMonitoringTimerAsync();
        }
    }
}
