using MQTTnet.Diagnostics.Logger;
using MQTTnet.Server;

namespace KegMonitor.Broker
{
    public class MqttLogger : IMqttNetLogger
    {
        private readonly ILogger<MqttServer> _logger;

        public MqttLogger(ILogger<MqttServer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsEnabled => true;

        public void Publish(MqttNetLogLevel level, string source, string message, object[] parameters, Exception exception)
        {
            _logger.Log(ConvertLogLevel(level), exception, message, parameters);
        }

        private static LogLevel ConvertLogLevel(MqttNetLogLevel logLevel)
        {
            switch (logLevel)
            {
                case MqttNetLogLevel.Error: return LogLevel.Error;
                case MqttNetLogLevel.Warning: return LogLevel.Warning;
                case MqttNetLogLevel.Info: return LogLevel.Information;
                case MqttNetLogLevel.Verbose: return LogLevel.Trace;
            }

            return LogLevel.Debug;
        }
    }
}
