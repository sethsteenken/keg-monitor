using KegMonitor.Core.Interfaces;

namespace KegMonitor.Web.Application
{
    public class ScaleWeightHandler : IScaleWeightHandler
    {
        private readonly IScaleUpdater _scaleUpdater;
        private readonly IEnumerable<IScaleWeightChangeNotifier> _weightChangeNotifiers;
        private readonly IEnumerable<IPourNotifier> _pourNotifiers;
        private readonly ILogger<ScaleWeightHandler> _logger;

        public ScaleWeightHandler(
            IScaleUpdater scaleUpdater,
            IEnumerable<IScaleWeightChangeNotifier> weightChangeNotifiers,
            IEnumerable<IPourNotifier> pourNotifiers,
            ILogger<ScaleWeightHandler> logger)
        {
            _scaleUpdater = scaleUpdater;
            _weightChangeNotifiers = weightChangeNotifiers;
            _pourNotifiers = pourNotifiers;
            _logger = logger;
        }

        public async Task HandleAsync(int scaleId, int weight)
        {
            var weightRecordingResult = await _scaleUpdater.UpdateAsync(scaleId, weight);

            _logger.LogInformation($"Scale {scaleId} - Update Result: Recorded?: {weightRecordingResult.Recorded} , Pour?: {weightRecordingResult.PourOccurred}");

            foreach (var notifier in _weightChangeNotifiers)
            {
                _logger.LogInformation($"Scale {scaleId} - Notifying weight change - {notifier.GetType().Name}...");
                await notifier.NotifyAsync(scaleId, weight);
            }

            if (weightRecordingResult.PourOccurred)
            {
                foreach (var pourNotifier in _pourNotifiers)
                {
                    _logger.LogInformation($"Scale {scaleId} - Notifying new pour - {pourNotifier.GetType().Name}...");
                    await pourNotifier.NotifyAsync(scaleId);
                }
            }
        }
    }
}
