using KegMonitor.Core.Interfaces;

namespace KegMonitor.Broker
{ 
    public class ScaleWeightMessageHandler : IScaleWeightHandler
    {
        private readonly string _domain;
        private readonly ILogger<ScaleWeightMessageHandler> _logger;

        public ScaleWeightMessageHandler(
            string domain,
            ILogger<ScaleWeightMessageHandler> logger)
        {
            _domain = domain;
            _logger = logger;
        }

        public async Task HandleAsync(int scaleId, int weight)
        {
            try
            {
                using var client = new HttpClient();
                string url = $"{_domain.TrimEnd('/')}/scale/weight/?id={scaleId}&w={weight}";
                _logger.LogInformation($"Sending request to web app: '{url}'");
                await client.PostAsync(url, new StringContent(string.Empty));
                _logger.LogInformation("Request completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending web request on weight change.");
            }
        }
    }
}
