using KegMonitor.Core.Interfaces;
using KegMonitor.Infrastructure.EntityFramework;
using MQTTnet;
using MQTTnet.Server;
using System.Net;

namespace KegMonitor.Server
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKegMonitorServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthSettings>(configuration.GetSection("Auth"));

            services.AddKegMonitorDataAccess(configuration);

            services.AddSingleton<IScaleWeightHandler, ScaleWeightHandler>();

            services.AddSingleton<IMqttServerConnectionValidator, ConnectionValidator>();
            services.AddSingleton<IMqttServerApplicationMessageInterceptor, ApplicationMessageInterceptor>();
            services.AddSingleton<IMqttServer>(serviceProvider => new MqttFactory().CreateMqttServer());
            services.AddSingleton<MqttServerOptionsBuilder>(serviceProvider =>
            {
                return new MqttServerOptionsBuilder()
                    .WithDefaultEndpointBoundIPAddress(IPAddress.Parse(configuration["Host:IpAddress"]))
                    .WithDefaultEndpointPort(int.Parse(configuration["Host:Port"]))
                    .WithConnectionValidator(serviceProvider.GetRequiredService<IMqttServerConnectionValidator>())
                    .WithApplicationMessageInterceptor(serviceProvider.GetRequiredService<IMqttServerApplicationMessageInterceptor>());
            });

            services.AddSingleton<IMqttServerOptions>(serviceProvider => serviceProvider.GetRequiredService<MqttServerOptionsBuilder>().Build());



            return services;
        }
    }
}
