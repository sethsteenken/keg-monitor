using KegMonitor.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace KegMonitor.Core.Services
{
    public class WebUIPourNotifier : IPourNotifier
    {
        private readonly string _domain;
        private readonly ILogger<WebUIPourNotifier> _logger;

        public WebUIPourNotifier(
            string domain,
            ILogger<WebUIPourNotifier> logger)
        {
            _domain = domain;
            _logger = logger;
        }

        public async Task NotifyAsync(int scaleId)
        {
            try
            {
                using var client = new HttpClient();
                string url = $"{_domain.TrimEnd('/')}/scale/pour/{scaleId}/";
                _logger.LogInformation($"Sending request to web app: '{url}'");
                await client.PostAsync(url, new StringContent(string.Empty));
                _logger.LogInformation("Request completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending web request on pour notifier.");
            }
        }
    }
}
