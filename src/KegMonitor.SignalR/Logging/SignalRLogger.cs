using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KegMonitor.SignalR
{
    public class SignalRLogger : ILogger
    {
        private readonly string _name;
        private readonly IServiceProvider _serviceProvider;
        //private readonly HubConnectionFactory _hubConnectionFactory;

        //public SignalRLogger(string name, HubConnectionFactory hubConnectionFactory)
        //{
        //    _name = name;
        //    _hubConnectionFactory = hubConnectionFactory;
        //}

        public SignalRLogger(string name, IServiceProvider serviceProvider)
        {
            _name = name;
            _serviceProvider = serviceProvider;
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                var message = formatter(state, exception);

                var hub = _serviceProvider.GetService<IHubContext<LogHub>>();
                if (hub != null)
                    hub.Clients.All.SendAsync("ReceiveMessage", _name, logLevel.ToString(), message).Wait();
                else
                    System.Diagnostics.Debug.WriteLine(" **** Hub is null.");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error - " + ex.ToString());
            }
        }
    }
}
