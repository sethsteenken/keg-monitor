using Microsoft.Extensions.Diagnostics.HealthChecks;
using MQTTnet.Extensions.ManagedClient;

namespace KegMonitor.Web.Application
{
    public class MqttConnectionHealthCheck : IHealthCheck
    {
        private readonly IManagedMqttClient _mqttClient;

        public MqttConnectionHealthCheck(IManagedMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _mqttClient.PingAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}
