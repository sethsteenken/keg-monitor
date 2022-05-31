using KegMonitor.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KegMonitor.Infrastructure.EntityFramework
{
    public class ScaleWeightHandler : IScaleWeightHandler
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;
        private readonly ILogger<ScaleWeightHandler> _logger;
        private readonly int _recordingThreshold = 100; // TODO - pass in / configure

        public ScaleWeightHandler(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory,
            ILogger<ScaleWeightHandler> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task HandleAsync(int scaleId, int weight)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var scale = await context.Scales.FirstOrDefaultAsync(s => s.Id == scaleId);
            if (scale == null)
            {
                _logger.LogError($"Scale ({scaleId}) not found.");
                return;
            }

            var difference = Math.Abs(scale.CurrentWeight - weight);

            if (difference > _recordingThreshold)
            {
                scale.UpdateWeight(weight);
                await context.SaveChangesAsync();
            }
            else
                _logger.LogDebug($"Weight difference {difference} less than threshold {_recordingThreshold}.");
        }
    }
}
