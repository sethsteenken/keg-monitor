using KegMonitor.Core.Entities;
using KegMonitor.Web.Application;
using MudBlazor;

namespace KegMonitor.Web
{
    public static class ChartSeriesExtensions
    {
        public static ScaleChart BuildScaleChart(
            this ChartSeries chartSeries, 
            int scaleId, 
            IEnumerable<ScaleWeightChange> scaleWeightChanges,
            int numOfWeightChangesRequested)
        {
            chartSeries.Name = $"Scale {scaleId}";
            chartSeries.Data = new double[numOfWeightChangesRequested];

            var percentages = new List<double>();

            for (int i = 0; i < numOfWeightChangesRequested; i++)
            {
                var weightChange = scaleWeightChanges.ElementAtOrDefault(i);

                if (weightChange != null)
                {
                    percentages.Add((double)weightChange.Scale.CalculatePercentage(weightChange.Weight));
                }
                else
                {
                    percentages.Add(0);
                    chartSeries.Data[i] = 0;
                }
            }

            var min = percentages.Min();

            for (int i = 0; i < percentages.Count; i++)
            {
                chartSeries.Data[i] = percentages[i] - (min - 1);
            }

            var chart = new ScaleChart()
            {
                Data = chartSeries,
                XLabels = new string[numOfWeightChangesRequested]
            };

            for (int i = 0; i < numOfWeightChangesRequested; i++)
            {
                chart.XLabels[i] = string.Empty;
            }

            return chart;
        }
    }
}
