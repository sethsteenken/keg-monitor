using KegMonitor.Core.Interfaces;
using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class ScaleWeightHandler : IScaleWeightHandler
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;
        private readonly IEnumerable<IScaleWeightChangeNotifier> _weightChangeNotifiers;
        private readonly IEnumerable<IPourNotifier> _pourNotifiers;
        private readonly ILogger<ScaleWeightHandler> _logger;

        public ScaleWeightHandler(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory,
            IEnumerable<IScaleWeightChangeNotifier> weightChangeNotifiers,
            IEnumerable<IPourNotifier> pourNotifiers,
            ILogger<ScaleWeightHandler> logger)
        {
            _dbContextFactory = dbContextFactory;
            _weightChangeNotifiers = weightChangeNotifiers;
            _pourNotifiers = pourNotifiers;
            _logger = logger;
        }

        public async Task HandleAsync(string topic, int weight)
        {
            if (string.IsNullOrEmpty(topic))
            {
                _logger.LogWarning("Topic is empty.");
                return;
            }
                
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            var scale = await context.Scales.Include(s => s.Beer)
                                            .SingleOrDefaultAsync(s => s.Topic == topic);
            if (scale == null)
            {
                _logger.LogError("Scale with topic '{topic}' not found.", topic);
                return;
            }

            var weightRecordingResult = scale.UpdateWeight(weight);

            await context.SaveChangesAsync();

            _logger.LogInformation($"Scale {scale.Id} - Pour?: {weightRecordingResult.PourOccurred}");

            foreach (var notifier in _weightChangeNotifiers)
            {
                _logger.LogInformation($"Scale {scale.Id} - Notifying weight change - {notifier.GetType().Name}...");
                await notifier.NotifyAsync(scale, weight);
            }

            if (weightRecordingResult.PourOccurred)
            {
                foreach (var pourNotifier in _pourNotifiers)
                {
                    _logger.LogInformation($"Scale {scale.Id} - Notifying new pour - {pourNotifier.GetType().Name}...");
                    await pourNotifier.NotifyAsync(scale);
                }
            }
        }
    }
}
