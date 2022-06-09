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

                if (!scale.Active || scale.Beer is null)
                {
                    _logger.LogInformation($"Scale {scaleId} currently inactive. Weight change recordings disabled.");
                    return new ScaleUpdateResult();
                }

                var difference = scale.Difference(weight);

                if (difference > scale.MaxThreshold)
                {
                    _logger.LogWarning($"Scale {scaleId} weight difference of {difference} exceeds maximum threshold of {scale.MaxThreshold}.");
                    return new ScaleUpdateResult();
                }

                if (difference > scale.RecordingDifferenceThreshold)
                {
                    scale.UpdateWeight(weight);
                    await context.SaveChangesAsync();

                    return new ScaleUpdateResult(
                        Recorded: true, 
                        PourOccurred: difference > scale.PourDifferenceThreshold);
                }
                else
                    _logger.LogDebug($"Weight difference {difference} less than Scale's ({scaleId}) threshold {scale.RecordingDifferenceThreshold}.");

                return new ScaleUpdateResult();
            }
        }
    }
}
