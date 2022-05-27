using KegMonitor.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KegMonitor.Core.Services
{
    public class ScaleWeightHandler : IScaleWeightHandler
    {
        private readonly IScaleRepository _scaleRepository;
        private readonly ILogger<ScaleWeightHandler> _logger;
        private readonly int _recordingThreshold;

        public ScaleWeightHandler(
            IScaleRepository scaleRepository, 
            ILogger<ScaleWeightHandler> logger,
            int recordingThreshold)
        {
            _scaleRepository = scaleRepository;
            _logger = logger;
            _recordingThreshold = recordingThreshold;
        }

        public async Task HandleAsync(int scaleId, int weight)
        {
            var scale = await _scaleRepository.GetByIdAsync(scaleId);
            if (scale == null)
            {
                _logger.LogError($"Scale ({scaleId}) not found.");
                return;
            }

            int difference = Math.Abs(scale.CurrentWeight - weight);

            if (difference > _recordingThreshold)
                scale.UpdateWeight(weight);
            else
                _logger.LogDebug($"Weight difference {difference} less than threshold {_recordingThreshold}.");
            
            await _scaleRepository.AddOrUpdateAsync(scale);

            
        }
    }
}
