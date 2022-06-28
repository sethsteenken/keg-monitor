using KegMonitor.Core;
using KegMonitor.Core.Interfaces;
using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class ScaleWeightUpdater : IScaleUpdater
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;
        private readonly ILogger<ScaleWeightUpdater> _logger;

        public ScaleWeightUpdater(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory,
            ILogger<ScaleWeightUpdater> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<ScaleUpdateResult> UpdateAsync(int scaleId, int weight)
        {
            await using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                var scale = await context.Scales.Include(s => s.Beer)
                                                .FirstOrDefaultAsync(s => s.Id == scaleId);
                if (scale == null)
                {
                    _logger.LogError($"Scale ({scaleId}) not found.");
                    return new ScaleUpdateResult();
                }

                var result = scale.UpdateWeight(weight);
                await context.SaveChangesAsync();

                return result;
            }
        }
    }
}
