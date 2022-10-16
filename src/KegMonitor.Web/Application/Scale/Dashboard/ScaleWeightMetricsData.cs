using KegMonitor.Core;

namespace KegMonitor.Web.Application
{
    public class ScaleWeightMetricsData
    {
        private readonly IEnumerable<WeightChangeEvent> _weightChanges;

        public ScaleWeightMetricsData(IEnumerable<WeightChangeEvent> weightChanges, ScaleChart chart, int numWeightChanges)
        {
            _weightChanges = weightChanges;

            Chart = chart;
            NumWeightChanges = numWeightChanges;

            WeightChangesForDisplay = _weightChanges.OrderByDescending(swc => swc.TimeStamp)
                                                    .Take(20);
        }

        public IEnumerable<WeightChangeEvent> WeightChangesForDisplay { get; }
        public ScaleChart Chart { get; }

        public int NumWeightChanges { get; }
        public int CurrentWeight => WeightChangesForDisplay.Select(wc => wc.Weight).FirstOrDefault();
        public int Min => _weightChanges.Select(wc => wc.Weight).Min();
        public int Max => _weightChanges.Select(wc => wc.Weight).Max();
        public decimal Average => (decimal)Math.Round(_weightChanges.Average(wc => (decimal)wc.Weight), 0);
    }
}
