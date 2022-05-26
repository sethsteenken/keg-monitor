using MQTTnet.Server;
using System.Text;

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

            if (context.ApplicationMessage == null)
            {
                context.CloseConnection = true;
            }
            else if (context.ApplicationMessage.IsSensorMessage())
            {
                _logger.LogDebug($"New Message - TimeStamp: {DateTime.Now} -- Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage.Topic}, Payload = {Encoding.UTF8.GetString(context.ApplicationMessage.Payload)}, QoS = {context.ApplicationMessage.QualityOfServiceLevel}, Retain-Flag = {context.ApplicationMessage.Retain}");

                if (context.ApplicationMessage.TryGetKegNumber(out int kegNumber))
                    _logger.LogDebug($" -- Keg Number: {kegNumber}");

                var payload = System.Text.Json.JsonSerializer.Deserialize<SensorPayload>(context.ApplicationMessage.Payload);
                if (payload != null)
                    _logger.LogDebug($" -- Payload: {payload.Time} - {payload.HX711.WeightRaw}");
            }

            return Task.CompletedTask;
        }
    }
}
