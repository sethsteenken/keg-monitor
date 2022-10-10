using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace KegMonitor.Web.Application
{
    public class ScaleDashboardQueryService : IScaleDashboardQueryService
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;

        public ScaleDashboardQueryService(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<ScaleDashboardModel> BuildModelAsync(int numOfWeightChanges)
        {
            if (numOfWeightChanges <= 0)
                throw new ArgumentOutOfRangeException(nameof(numOfWeightChanges));

            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var scales = await context.Scales.OrderBy(s => s.Id).Include(s => s.Beer).ToListAsync();

            var model = new ScaleDashboardModel()
            {
                Scales = scales.Select(s => new ScaleDashboardItem()
                {
                    Id = s.Id,
                    Weight = s.CurrentWeight,
                    Percentage = (double)s.Percentage,
                    Active = s.Active,
                    Beer = s.Beer == null ? null : new BeerDisplayItem()
                    {
                        Name = s.Beer.Name,
                        Type = s.Beer.Type,
                        ABV = s.Beer.ABV,
                        Description = s.Beer.Description,
                        ImagePath = s.Beer.ImagePath
                    }
                }).ToList()
            };

            foreach (var scale in model.Scales)
            {
                scale.Chart = await GetScaleChartInternalAsync(context, scale.Id, numOfWeightChanges);
            }

            return model;
        }

        public async Task<ScaleChart> GetScaleChartAsync(int scaleId, int numOfWeightChanges)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            return await GetScaleChartInternalAsync(context, scaleId, numOfWeightChanges);
        }

        private async Task<ScaleChart> GetScaleChartInternalAsync(KegMonitorDbContext context, int scaleId, int numOfWeightChanges)
        {
            var scaleWeightChanges = await context.ScaleWeightChanges.Include(s => s.Scale)
                                                                    .Where(swc => swc.Scale.Id == scaleId)
                                                                    .OrderByDescending(swc => swc.TimeStamp)
                                                                    .Take(numOfWeightChanges)
                                                                    .ToListAsync();
            var chartSeries = new ChartSeries()
            {
                Name = $"Scale {scaleId}",
                Data = new double[numOfWeightChanges]
            };

            var percentages = new List<double>();

            for (int i = 0; i < numOfWeightChanges; i++)
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
                XLabels = new string[numOfWeightChanges]
            };

            for (int i = 0; i < numOfWeightChanges; i++)
            {
                chart.XLabels[i] = string.Empty;
            }

            return chart;
        }

        public async Task<List<int>> GetScaleIdsAsync()
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Scales.OrderBy(s => s.Id).Select(s => s.Id).ToListAsync();
        }
    }
}
