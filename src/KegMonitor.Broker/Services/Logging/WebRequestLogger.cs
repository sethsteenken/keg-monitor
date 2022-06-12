using KegMonitor.Core;

namespace KegMonitor.Broker
{
    public class WebRequestLogger : ILogger, IDisposable
    {
        private readonly string _name;
        private readonly string _url;
        private HttpClient _httpClient;

        public WebRequestLogger(string name, string url)
        {
            _name = name;
            _url = url;
            _httpClient = new HttpClient();
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = new LogMessage()
            {
                Level = logLevel.ToString(),
                Logger = _name,
                Message = formatter(state, exception)
            };

            try
            {
                _httpClient.PostAsJsonAsync(_url, message).Wait();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error sending log message to " + _url + " - " + ex.ToString());
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
