using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KegMonitor.SignalR
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerProvider>(serviceProvider => new SignalRLoggerProvider(serviceProvider));

            return services;
        }
    }
}
