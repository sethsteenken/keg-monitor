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
            var payload = context.ApplicationMessage?.Payload == null ?
                               null : Encoding.UTF8.GetString(context.ApplicationMessage.Payload);

            if (context.ApplicationMessage == null)
            {
                context.CloseConnection = true;
            }
            else if (context.ApplicationMessage.IsSensorMessage())
            {
                _logger.LogInformation($"New Message - TimeStamp: {DateTime.Now} -- Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage.Topic}, Payload = {payload}, QoS = {context.ApplicationMessage.QualityOfServiceLevel}, Retain-Flag = {context.ApplicationMessage.Retain}");

                if (context.ApplicationMessage.TryGetKegNumber(out int kegNumber))
                    _logger.LogInformation($" -- Keg Number: {kegNumber}");
            }

            return Task.CompletedTask;
        }
    }
}
