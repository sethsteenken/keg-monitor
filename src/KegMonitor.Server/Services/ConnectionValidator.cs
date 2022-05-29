using Microsoft.Extensions.Options;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace KegMonitor.Server
{
    internal class ConnectionValidator : IMqttServerConnectionValidator
    {
        private readonly ILogger<ConnectionValidator> _logger;
        private readonly AuthSettings _authSettings;

        public ConnectionValidator(
            ILogger<ConnectionValidator> logger,
            IOptions<AuthSettings> authSettings)
        {
            _logger = logger;
            _authSettings = authSettings.Value;
        }

        public Task ValidateConnectionAsync(MqttConnectionValidatorContext context)
        {
            if (!_authSettings.Enabled)
            {
                context.ReasonCode = MqttConnectReasonCode.ServerUnavailable;
                _logger.LogDebug($"Connection denied. Server not set to enable auth. ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");
                return Task.CompletedTask;
            }

            if (!_authSettings.AllowedClientIds?.Contains(context.ClientId) ?? false)
            {
                context.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                _logger.LogWarning($"FAILED connection. Client not allowed: ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");
                return Task.CompletedTask;
            }

            _logger.LogDebug($"New valid connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");

            return Task.CompletedTask;
        }
    }
}
