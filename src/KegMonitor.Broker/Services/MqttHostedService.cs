using Microsoft.Extensions.Options;
using MQTTnet.Server;

namespace KegMonitor.Broker
{
    public class MqttHostedService : BackgroundService
    {
        private readonly IMqttServer _mqttServer;
        private readonly IMqttServerOptions _mqttServerOptions;
        private readonly MqttServerSettings _settings;
        private readonly ILogger<MqttHostedService> _logger;

        public MqttHostedService(
            IMqttServer mqttServer, 
            IMqttServerOptions mqttServerOptions,
            IOptions<MqttServerSettings> settings,
            ILogger<MqttHostedService> logger)
        {
            _mqttServer = mqttServer;
            _mqttServerOptions = mqttServerOptions;
            _settings = settings.Value;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting {_settings.ServiceName} {GetType().Name} service...");

            await _mqttServer.StartAsync(_mqttServerOptions);

            _logger.LogInformation($"Service {_settings.ServiceName} {GetType().Name} started.");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogMemoryInformation(_settings.ServiceName);
                    await Task.Delay(_settings.HeartbeatDelay, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred: {Exception}", ex);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping {_settings.ServiceName} {GetType().Name} service...");

            await _mqttServer.StopAsync();

            _logger.LogInformation($"Service {_settings.ServiceName} {GetType().Name} stopped.");

            await base.StopAsync(cancellationToken);
        }
    }
}
