using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KegMonitor.SignalR
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRLogging(this IServiceCollection services, string domain)
        {
            //services.AddSingleton(serviceProvider => new HubConnectionFactory(domain));
            //services.AddSingleton<ILoggerProvider>(serviceProvider =>
            //{
            //    return new SignalRLoggerProvider(domain);
            //});

            services.AddSingleton<ILoggerProvider>(serviceProvider => new SignalRLoggerProvider(serviceProvider));

            return services;
        }
    }
}
