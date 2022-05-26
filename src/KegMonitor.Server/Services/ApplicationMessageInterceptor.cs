using MQTTnet.Server;
using System.Text;

namespace KegMonitor.Server
{
    internal class ApplicationMessageInterceptor : IMqttServerApplicationMessageInterceptor
    {
        public ApplicationMessageInterceptor()
        {

        }

        public Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
        {
            var payload = context.ApplicationMessage?.Payload == null ?
                               null : Encoding.UTF8.GetString(context.ApplicationMessage.Payload);

            if (context.ApplicationMessage?.Topic == "tele/keg1/SENSOR")
            {
                Console.WriteLine($"New Message - TimeStamp: {DateTime.Now} -- Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage?.Topic}, Payload = {payload}, QoS = {context.ApplicationMessage?.QualityOfServiceLevel}, Retain-Flag = {context.ApplicationMessage?.Retain}");
            }

            return Task.CompletedTask;
        }
    }
}
