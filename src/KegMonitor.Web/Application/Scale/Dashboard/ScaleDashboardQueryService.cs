using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class ScaleDashboardQueryService : IScaleDashboardQueryService
    {
        private const int _defaultNumWeightChanges = 20;
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;

        public ScaleDashboardQueryService(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<ScaleDashboardModel> BuildModelAsync()
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var scales = await context.Scales.OrderBy(s => s.Id)
                                             .Include(s => s.Beer)
                                             .Include(s => s.WeightChanges.OrderByDescending(wc => wc.TimeStamp).Take(_defaultNumWeightChanges))
                                             .ToListAsync();

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
                        OG = s.Beer.OG,
                        FG = s.Beer.FG,
                        IBU = s.Beer.IBU,
                        TapDate = s.Beer.TapDate?.ToShortDateString(),
                        Description = s.Beer.Description
                    },
                    WeightMetricsData = new ScaleWeightMetricsData(s, _defaultNumWeightChanges)
                }).ToList()
            };

            return model;
        }

        public async Task<ScaleWeightMetricsData> GetWeightMetricsAsync(int scaleId, int numOfWeightChanges)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var scale = await context.Scales.Include(s => s.WeightChanges.OrderByDescending(wc => wc.TimeStamp).Take(numOfWeightChanges))
                                           .SingleAsync();

            return new ScaleWeightMetricsData(scale, numOfWeightChanges);
        }

        public async Task<List<int>> GetScaleIdsAsync()
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Scales.OrderBy(s => s.Id).Select(s => s.Id).ToListAsync();
        }
    }
}
