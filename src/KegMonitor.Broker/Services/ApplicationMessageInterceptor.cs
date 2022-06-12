using KegMonitor.Core.Interfaces;
using MQTTnet.Server;
using System.Text;
using System.Text.Json;

namespace KegMonitor.Broker
{
    internal class ApplicationMessageInterceptor : IMqttServerApplicationMessageInterceptor
    {
        private readonly ILogger<ApplicationMessageInterceptor> _logger;
        private readonly IScaleWeightHandler _scaleWeightHandler;

        public ApplicationMessageInterceptor(
            ILogger<ApplicationMessageInterceptor> logger,
            IScaleWeightHandler scaleWeightHandler)
        {
            _logger = logger;
            _scaleWeightHandler = scaleWeightHandler;
        }

        public async Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
        {
            if (context == null)
                return;

            _logger.LogDebug($"New Message - TimeStamp: {DateTime.Now} -- Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage.Topic}, Payload = {Encoding.UTF8.GetString(context.ApplicationMessage.Payload)}, QoS = {context.ApplicationMessage.QualityOfServiceLevel}, Retain-Flag = {context.ApplicationMessage.Retain}");

            if (context.ApplicationMessage == null)
            {
                context.CloseConnection = true;
            }
            else if (context.ApplicationMessage.IsSensorMessage() && context.ApplicationMessage.TryGetScaleNumber(out int scaleNumber))
            {
                _logger.LogInformation($"Scale Number: {scaleNumber}");

                var payload = JsonSerializer.Deserialize<SensorPayload>(context.ApplicationMessage.Payload);
                if (payload != null)
                {
                    _logger.LogInformation($"Payload: {payload.Time} - {payload.HX711.WeightRaw}");
                    await _scaleWeightHandler.HandleAsync(scaleNumber, payload.HX711.WeightRaw);
                }
            }
        }
    }
}
