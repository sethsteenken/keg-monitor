using KegMonitor.Core.Entities;
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

        public async Task<ScaleDashboardModel> BuildModelAsync()
        {
            int numToInclude = 200;
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var scales = await context.Scales.OrderBy(s => s.Id).Include(s => s.Beer).ToListAsync();

            var model = new ScaleDashboardModel()
            {
                Scales = scales.Select(s => new ScaleDisplayItem()
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
                }),
                ChartXLabels = new string[numToInclude]
            };

            var weightChanges = new List<ScaleWeightChange>();

            foreach (var scale in model.Scales)
            {
                var scaleWeightChanges = await context.ScaleWeightChanges.Include(s => s.Scale)
                                                                         .Where(swc => swc.Scale.Id == scale.Id)
                                                                         .OrderByDescending(swc => swc.TimeStamp)
                                                                         .Take(numToInclude)
                                                                         .ToListAsync();

                if (scaleWeightChanges.Any())
                    weightChanges.AddRange(scaleWeightChanges);

                model.ChartSeries.Add(new ChartSeries()
                {
                    Name = scale.Id.ToString(),
                    Data = new double[numToInclude]
                });
            }

            for (int i = 0; i < numToInclude; i++)
            {
                DateTime? timestamp = null;
                var timeStampXLabel = "--";

                foreach (var chart in model.ChartSeries)
                {
                    var weightChange = weightChanges.Where(wc => wc.Scale.Id == int.Parse(chart.Name))
                                                    .OrderByDescending(wc => wc.TimeStamp)
                                                    .ElementAtOrDefault(i);

                    if (weightChange != null)
                    {
                        if (timestamp == null)
                            timestamp = weightChange.TimeStamp;
                        else if (weightChange.TimeStamp > timestamp)
                            timestamp = weightChange.TimeStamp;

                        chart.Data[i] = (double)weightChange.Scale.CalculatePercentage(weightChange.Weight);
                    }
                    else
                    {
                        chart.Data[i] = 0;
                    }
                }

                if (timestamp != null)
                    timeStampXLabel = timestamp.Value.RoundUp(TimeSpan.FromSeconds(10)).ToString("MM/dd mm:HH:ss");

                model.ChartXLabels[i] = String.Empty;
            }

            foreach (var chart in model.ChartSeries)
            {
                chart.Name = $"Scale " + chart.Name;
            }

            return model;
        }

        public async Task<List<int>> GetScaleIdsAsync()
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Scales.OrderBy(s => s.Id).Select(s => s.Id).ToListAsync();
        }

        public async Task<List<ScaleDisplayItem>> GetScalesAsync()
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var scales = await context.Scales.OrderBy(s => s.Id).Include(s => s.Beer).ToListAsync();

            return scales.Select(s => new ScaleDisplayItem()
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
            }).ToList();
        }
    }
}
