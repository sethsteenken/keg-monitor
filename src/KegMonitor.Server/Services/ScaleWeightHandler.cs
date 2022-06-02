using KegMonitor.Core.Interfaces;
using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Server
{
    public class ScaleWeightHandler : IScaleWeightHandler
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;
        private readonly ILogger<ScaleWeightHandler> _logger;

        public ScaleWeightHandler(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory,
            ILogger<ScaleWeightHandler> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task HandleAsync(int scaleId, int weight)
        {
            await using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                var scale = await context.Scales.Include(s => s.Beer)
                                                .FirstOrDefaultAsync(s => s.Id == scaleId);
                if (scale == null)
                {
                    _logger.LogError($"Scale ({scaleId}) not found.");
                    return;
                }

                if (!scale.Active || scale.Beer is null)
                {
                    _logger.LogInformation($"Scale {scaleId} currently inactive. Weight change recordings disabled.");
                    return;
                }

                var difference = Math.Abs(scale.CurrentWeight - weight);

                if (difference > scale.RecordingDifferenceThreshold)
                {
                    scale.UpdateWeight(weight);
                    await context.SaveChangesAsync();
                }
                else
                    _logger.LogDebug($"Weight difference {difference} less than Scale's ({scaleId}) threshold {scale.RecordingDifferenceThreshold}.");
            }
        }
    }
}
