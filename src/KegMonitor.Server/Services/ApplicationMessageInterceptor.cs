using MQTTnet.Server;
using System.Text;
using System.Text.Json;

namespace KegMonitor.Server
{
    internal class ApplicationMessageInterceptor : IMqttServerApplicationMessageInterceptor
    {
        private readonly ILogger<ApplicationMessageInterceptor> _logger;

        public ApplicationMessageInterceptor(ILogger<ApplicationMessageInterceptor> logger)
        {
            _logger = logger;
        }

        public Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
        {
            if (context == null)
                return Task.CompletedTask;

            _logger.LogTrace($"New Message - TimeStamp: {DateTime.Now} -- Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage.Topic}, Payload = {Encoding.UTF8.GetString(context.ApplicationMessage.Payload)}, QoS = {context.ApplicationMessage.QualityOfServiceLevel}, Retain-Flag = {context.ApplicationMessage.Retain}");

            if (context.ApplicationMessage == null)
            {
                context.CloseConnection = true;
            }
            else if (context.ApplicationMessage.IsSensorMessage() && context.ApplicationMessage.TryGetScaleNumber(out int scaleNumber))
            {
                _logger.LogDebug($"Scale Number: {scaleNumber}");

                var payload = JsonSerializer.Deserialize<SensorPayload>(context.ApplicationMessage.Payload);
                if (payload != null)
                {
                    _logger.LogDebug($"Payload: {payload.Time} - {payload.HX711.WeightRaw}");
                }  
            }

            return Task.CompletedTask;
        }
    }
}
