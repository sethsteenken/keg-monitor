using MQTTnet.Protocol;
using MQTTnet.Server;

namespace KegMonitor.Server
{
    internal class ConnectionValidator : IMqttServerConnectionValidator
    {
        private readonly ILogger<ConnectionValidator> _logger;

        public ConnectionValidator(ILogger<ConnectionValidator> logger)
        {
            _logger = logger;
        }

        public Task ValidateConnectionAsync(MqttConnectionValidatorContext context)
        {
            if (context.ClientId != "DVES_50B067")
            {
                context.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                _logger.LogWarning($"FAILED connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");
            }

            _logger.LogInformation($"New valid connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");

            return Task.CompletedTask;
        }
    }
}
