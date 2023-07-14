using KegMonitor.Core;

namespace KegMonitor.Web.Application
{
    public class ScaleWeightMetricsData
    {
        private readonly IEnumerable<WeightChangeEvent> _weightChanges;

        public ScaleWeightMetricsData(IEnumerable<WeightChangeEvent> weightChanges, ScaleChart chart, int numWeightChanges)
        {
            _weightChanges = weightChanges ?? new List<WeightChangeEvent>();

            Chart = chart;
            NumWeightChanges = numWeightChanges;

            WeightChangesForDisplay = _weightChanges.OrderByDescending(swc => swc.TimeStamp)
                                                    .Take(20);

            Count = _weightChanges.Count();
            Min = _weightChanges.Any() ? _weightChanges.Select(wc => wc.Weight).Min() : 0;
            Max = _weightChanges.Any() ? _weightChanges.Select(wc => wc.Weight).Max() : 0;
            Average = _weightChanges.Any() ? (decimal)Math.Round(_weightChanges.Average(wc => (decimal)wc.Weight), 0) : 0;
        }

        public IEnumerable<WeightChangeEvent> WeightChangesForDisplay { get; }
        public ScaleChart Chart { get; }

        public int NumWeightChanges { get; }
        public int CurrentWeight => WeightChangesForDisplay.Select(wc => wc.Weight).FirstOrDefault();
        public int Count { get; private set; }
        public int Min { get; private set; }
        public int Max { get; private set; }
        public decimal Average { get; private set; }

        public SensorStatusOption SensorStatus
        {
            get
            {
                if (WeightChangesForDisplay == null)
                    return SensorStatusOption.Unknown;

                if (WeightChangesForDisplay.Any())
                {
                    var latestTimeStamp = WeightChangesForDisplay.Select(w => w.TimeStamp).First();

                    if (latestTimeStamp >= DateTime.Now.AddSeconds(-10))
                        return SensorStatusOption.Online;

                    if (latestTimeStamp < DateTime.Now.AddSeconds(-20))
                        return SensorStatusOption.Offline;
                }

                return SensorStatusOption.Unknown;
            }
        }
    }
}
