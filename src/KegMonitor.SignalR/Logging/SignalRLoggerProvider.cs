using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace KegMonitor.SignalR
{
    public class SignalRLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly IServiceProvider _serviceProvider;

        public SignalRLoggerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new SignalRLogger(name, _serviceProvider));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
