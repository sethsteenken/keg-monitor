using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class SensorManager : ISensorManager
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;
        private readonly ILogger<SensorManager> _logger;
        private readonly HttpClient _httpClient;
        private const int _period = 10;

        public SensorManager(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory,
            ILogger<SensorManager> logger,
            HttpClient httpClient)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<SensorUpdateResult> BringOnlineAsync(int scaleId)
        {
            try
            {
                await using var context = await _dbContextFactory.CreateDbContextAsync();
                var scale = await context.Scales.FirstOrDefaultAsync(x => x.Id == scaleId);
                if (scale == null)
                    return SensorUpdateResult.Failed("Scale not found.");

                if (string.IsNullOrWhiteSpace(scale.Endpoint))
                    return SensorUpdateResult.Failed("Scale does not have a valid endpoint.");

                string url = FormatEndpointForCommands(scale.Endpoint, $"TelePeriod {_period}");
                var response = await _httpClient.GetAsync(url);

                var jsonResponse = await response.Content.ReadFromJsonAsync<SensorTelePeriodCommandResult>();
                if (jsonResponse.TelePeriod != _period)
                    return SensorUpdateResult.Failed($"Sensor failed to come online. TelePeriod set to {jsonResponse.TelePeriod}.");

                return SensorUpdateResult.Succeeded("Sensor has been brought online successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error bringing sensor online.", ex);
                return SensorUpdateResult.Failed(ex.Message);
            } 
        }

        public async Task<SensorUpdateResult> TakeOfflineAsync(int scaleId)
        {
            try
            {
                await using var context = await _dbContextFactory.CreateDbContextAsync();
                var scale = await context.Scales.FirstOrDefaultAsync(x => x.Id == scaleId);
                if (scale == null)
                    return SensorUpdateResult.Failed("Scale not found.");

                if (string.IsNullOrWhiteSpace(scale.Endpoint))
                    return SensorUpdateResult.Failed("Scale does not have a valid endpoint.");

                string url = FormatEndpointForCommands(scale.Endpoint, $"TelePeriod 0");
                var response = await _httpClient.GetAsync(url);

                var jsonResponse = await response.Content.ReadFromJsonAsync<SensorTelePeriodCommandResult>();
                if (jsonResponse.TelePeriod != 0)
                    return SensorUpdateResult.Failed($"Sensor failed to go offline. TelePeriod set to {jsonResponse.TelePeriod}.");

                return SensorUpdateResult.Succeeded("Sensor has been taken offline successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error taking sensor offline.", ex);
                return SensorUpdateResult.Failed(ex.Message);
            }
        }

        private static string FormatEndpointForCommands(string endpoint, string command)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException(nameof(endpoint));

            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            if (!endpoint.StartsWith("http"))
                endpoint = $"http://{endpoint}";

            return $"{endpoint.TrimEnd('/')}/cm?cmnd={command}";
        }
    }
}
