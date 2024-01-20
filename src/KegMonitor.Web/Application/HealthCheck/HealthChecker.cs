using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Extensions.ManagedClient;

namespace KegMonitor.Web.Application
{
    public class HealthChecker : IHealthChecker
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;
        private readonly IManagedMqttClient _mqttClient;
        private readonly ILogger<HealthChecker> _logger;

        public HealthChecker(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory, 
            IManagedMqttClient mqttClient,
            ILogger<HealthChecker> logger)
        {
            _dbContextFactory = dbContextFactory;
            _mqttClient = mqttClient;
            _logger = logger;
        }

        public async Task<bool> CheckAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Health Check - pinging MQTT broker...");
                await _mqttClient.PingAsync(cancellationToken);

                _logger.LogInformation("Health Check - testing database connection...");
                await using var dbContext = _dbContextFactory.CreateDbContext();
                await dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);

                _logger.LogInformation("Health Check - successful.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Health Check Failed - {ex.Message}");
                return false;
            }
        }
    }
}
