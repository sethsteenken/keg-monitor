using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
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
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;
        private readonly ILogger<MqttStartup> _logger;

        public MqttStartup(
            ManagedMqttClientOptions clientOptions,
            IManagedMqttClient mqttClient,
            IOptions<MqttBrokerSettings> settings,
            IDbContextFactory<KegMonitorDbContext> dbContextFactory,
            ILogger<MqttStartup> logger)
        {
            _clientOptions = clientOptions;
            _mqttClient = mqttClient;
            _settings = settings;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            var settings = _settings.Value;
            if (!settings.Subscribe)
                return;

            try
            {
                using var context = await _dbContextFactory.CreateDbContextAsync();
                var topics = await context.Scales.Where(s => !string.IsNullOrEmpty(s.Topic))
                                                 .Select(s => s.Topic)
                                                 .ToListAsync();

                if (!topics.Any())
                {
                    _logger.LogWarning("Failed to subscribe to MQTT broker: No Scales registred or no MQTT subscription Topics set on Scale data.");
                    return;
                }

                await _mqttClient.StartAsync(_clientOptions);

                var filters = topics.Select(t => new MqttTopicFilter() { Topic = t }).ToList();
                await _mqttClient.SubscribeAsync(filters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to MQTT broker. See exception for details.");
            }
        }
    }
}
