﻿namespace KegMonitor.Web.Application
{
    public class ScaleDashboardItem : ScaleDisplayItem
    {
        public ScaleWeightMetricsData WeightMetricsData { get; set; }
        public bool IsExpanded { get; set; } = false;
    }
}
