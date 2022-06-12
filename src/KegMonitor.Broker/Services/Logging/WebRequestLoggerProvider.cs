using System.Collections.Concurrent;

namespace KegMonitor.Broker
{
    public class WebRequestLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
        private readonly string _url;

        public WebRequestLoggerProvider(string url)
        {
            _url = url;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new WebRequestLogger(name, _url));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
