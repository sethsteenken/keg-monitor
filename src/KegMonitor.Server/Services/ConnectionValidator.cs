using MQTTnet.Protocol;
using MQTTnet.Server;

namespace KegMonitor.Server
{
    internal class ConnectionValidator : IMqttServerConnectionValidator
    {
        public ConnectionValidator()
        {

        }

        public Task ValidateConnectionAsync(MqttConnectionValidatorContext context)
        {
            if (context.ClientId != "DVES_50B067")
            {
                context.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                Console.WriteLine($"FAILED connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");
            }

            Console.WriteLine($"New valid connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");

            return Task.CompletedTask;
        }
    }
}
