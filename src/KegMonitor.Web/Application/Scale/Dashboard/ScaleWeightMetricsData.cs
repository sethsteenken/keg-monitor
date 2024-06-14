using KegMonitor.Core;
using KegMonitor.Core.Entities;
using MudBlazor;

namespace KegMonitor.Web.Application
{
    public class ScaleWeightMetricsData
    {
        private readonly IEnumerable<WeightChangeEvent> _weightChanges;

        public ScaleWeightMetricsData(Scale scale, int numOfWeightChangesRequested)
        {
            var chart = new ChartSeries().BuildScaleChart(scale.Id, scale.WeightChanges, numOfWeightChangesRequested);

            _weightChanges = scale.WeightChanges.Select(swc => new WeightChangeEvent(swc.Weight, swc.TimeStamp.ToLocalTime(), swc.IsPourEvent));

            Chart = chart;
            NumWeightChanges = numOfWeightChangesRequested;

            WeightChangesForDisplay = _weightChanges.OrderByDescending(swc => swc.TimeStamp)
                                                    .Take(20);

            Count = _weightChanges.Count();
            Min = _weightChanges.Any() ? _weightChanges.Select(wc => wc.Weight).Min() : 0;
            Max = _weightChanges.Any() ? _weightChanges.Select(wc => wc.Weight).Max() : 0;
            Average = _weightChanges.Any() ? (decimal)Math.Round(_weightChanges.Average(wc => (decimal)wc.Weight), 0) : 0;

            SensorStatus = scale.SensorOnline ? SensorStatusOption.Online : SensorStatusOption.Offline;
        }

        public IEnumerable<WeightChangeEvent> WeightChangesForDisplay { get; }
        public ScaleChart Chart { get; }

        public int NumWeightChanges { get; }
        public int CurrentWeight => WeightChangesForDisplay.Select(wc => wc.Weight).FirstOrDefault();
        public int Count { get; private set; }
        public int Min { get; private set; }
        public int Max { get; private set; }
        public decimal Average { get; private set; }

        public SensorStatusOption SensorStatus { get; private set; }
    }
}
