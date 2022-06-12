using Microsoft.Extensions.Options;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace KegMonitor.Broker
{
    internal class ConnectionValidator : IMqttServerConnectionValidator
    {
        private readonly ILogger<ConnectionValidator> _logger;
        private readonly MqttServerSettings _settings;

        public ConnectionValidator(
            ILogger<ConnectionValidator> logger,
            IOptions<MqttServerSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public Task ValidateConnectionAsync(MqttConnectionValidatorContext context)
        {
            if (!_settings.Enabled)
            {
                context.ReasonCode = MqttConnectReasonCode.ServerUnavailable;
                _logger.LogInformation($"Connection denied. Server not set to enable auth. ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");
                return Task.CompletedTask;
            }

            if (!_settings.AllowedClientIds?.Contains(context.ClientId) ?? false)
            {
                context.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                _logger.LogWarning($"FAILED connection. Client not allowed: ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");
                return Task.CompletedTask;
            }

            _logger.LogInformation($"New valid connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");

            return Task.CompletedTask;
        }
    }
}
