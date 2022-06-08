using KegMonitor.Core.Entities;
using KegMonitor.Core.Interfaces;
using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class ScaleWeightHandler : IScaleWeightHandler
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;
        private readonly IEnumerable<IPourNotifier> _pourNotifiers;
        private readonly ILogger<ScaleWeightHandler> _logger;

        public ScaleWeightHandler(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory,
            IEnumerable<IPourNotifier> pourNotifiers,
            ILogger<ScaleWeightHandler> logger)
        {
            _dbContextFactory = dbContextFactory;
            _pourNotifiers = pourNotifiers;
            _logger = logger;
        }

        public async Task HandleAsync(int scaleId, int weight)
        {
            Scale scale;
            bool pourOccurred = false;

            // TODO - signalr notify latest weight value

            await using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                scale = await context.Scales.Include(s => s.Beer)
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

                var difference = scale.Difference(weight);

                if (difference > scale.MaxThreshold)
                {
                    _logger.LogWarning($"Scale {scaleId} weight difference of {difference} exceeds maximum threshold of {scale.MaxThreshold}.");
                    return;
                }

                pourOccurred = difference > scale.PourDifferenceThreshold;

                if (difference > scale.RecordingDifferenceThreshold)
                {
                    scale.UpdateWeight(weight);
                    await context.SaveChangesAsync();
                }
                else
                    _logger.LogDebug($"Weight difference {difference} less than Scale's ({scaleId}) threshold {scale.RecordingDifferenceThreshold}.");
            }

            if (pourOccurred)
            {
                foreach (var pourNotifier in _pourNotifiers)
                {
                    await pourNotifier.NotifyAsync(scale.Id);
                }
            }
        }
    }
}
