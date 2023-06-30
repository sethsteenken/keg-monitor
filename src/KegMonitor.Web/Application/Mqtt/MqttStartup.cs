using Microsoft.Extensions.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;

namespace KegMonitor.Web.Application
{
    public class MqttStartup : IMqttStartup
    {
        private readonly ManagedMqttClientOptions _clientOptions;
        private readonly IManagedMqttClient _mqttClient;
        private readonly IOptions<MqttBrokerSettings> _settings;

        public MqttStartup(
            ManagedMqttClientOptions clientOptions,
            IManagedMqttClient mqttClient,
            IOptions<MqttBrokerSettings> settings)
        {
            _clientOptions = clientOptions;
            _mqttClient = mqttClient;
            _settings = settings;
        }

        public async Task SubscribeAsync()
        {
            var settings = _settings.Value;
            if (!settings.Subscribe)
                return;

            if (!settings.Topics.Any())
                throw new InvalidOperationException("No MQTT subscription topics set in configuration.");

            await _mqttClient.StartAsync(_clientOptions);

            var filters = settings.Topics.Select(t => new MqttTopicFilter() { Topic = t }).ToList();
            await _mqttClient.SubscribeAsync(filters);
        }
    }
}
