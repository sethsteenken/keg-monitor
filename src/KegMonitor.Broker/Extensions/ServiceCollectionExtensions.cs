using KegMonitor.Core.Interfaces;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.AspNetCore;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Implementations;
using MQTTnet.Server;
using System.Net;

namespace KegMonitor.Broker
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthSettings>(configuration.GetSection("Auth"));
            services.AddSingleton<IScaleWeightHandler>(serviceProvider =>
            {
                return new ScaleWeightMessageHandler(
                    serviceProvider.GetRequiredService<IConfiguration>()["WebUI:Domain"],
                    serviceProvider.GetRequiredService<ILogger<ScaleWeightMessageHandler>>());
            });

            services.AddSingleton<IMqttServerConnectionValidator, ConnectionValidator>()
                    .AddSingleton<IMqttServerApplicationMessageInterceptor, ApplicationMessageInterceptor>()
                    .AddSingleton<MqttWebSocketServerAdapter>()
                    .AddSingleton<IMqttNetLogger, MqttLogger>()
                    .AddSingleton<IMqttFactory>(serviceProvider =>
                    {
                        return new MqttFactory(serviceProvider.GetRequiredService<IMqttNetLogger>());
                    })
                    .AddSingleton<MqttServerOptionsBuilder>(serviceProvider =>
                    {
                        return new MqttServerOptionsBuilder()
                            .WithDefaultEndpointBoundIPAddress(IPAddress.Parse(configuration["Host:IpAddress"]))
                            .WithDefaultEndpointPort(int.Parse(configuration["Host:Port"]))
                            .WithConnectionValidator(serviceProvider.GetRequiredService<IMqttServerConnectionValidator>())
                            .WithApplicationMessageInterceptor(serviceProvider.GetRequiredService<IMqttServerApplicationMessageInterceptor>());
                    })
                    .AddSingleton<IMqttServerOptions>(serviceProvider => serviceProvider.GetRequiredService<MqttServerOptionsBuilder>().Build())
                    .AddSingleton<IMqttServer>(serviceProvider =>
                    {
                        var adapters = new List<IMqttServerAdapter>
                        {
                            new MqttTcpServerAdapter(serviceProvider.GetRequiredService<IMqttNetLogger>())
                            {
                                TreatSocketOpeningErrorAsWarning = true
                            },
                            serviceProvider.GetRequiredService<MqttWebSocketServerAdapter>()
                        };

                        return serviceProvider.GetRequiredService<IMqttFactory>().CreateMqttServer(adapters);
                    });

            return services;
        }
    }
}
