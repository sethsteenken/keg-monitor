using KegMonitor.Core.Interfaces;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Server;
using System.Net;

namespace KegMonitor.Broker
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MqttServerSettings>(configuration.GetSection("Mqtt"));

            services.AddSingleton<IMqttServerConnectionValidator, ConnectionValidator>()
                    .AddSingleton<IMqttServerApplicationMessageInterceptor, ApplicationMessageInterceptor>()
                    .AddSingleton<IMqttNetLogger, MqttLogger>()
                    .AddSingleton<IMqttFactory>(serviceProvider =>
                    {
                        return new MqttFactory(serviceProvider.GetRequiredService<IMqttNetLogger>());
                    })
                    .AddSingleton<MqttServerOptionsBuilder>(serviceProvider =>
                    {
                        var settings = serviceProvider.GetRequiredService<IOptions<MqttServerSettings>>().Value;

                        var builder = new MqttServerOptionsBuilder()
                            .WithDefaultEndpoint()
                            .WithDefaultEndpointPort(settings.Port)
                            .WithConnectionValidator(serviceProvider.GetRequiredService<IMqttServerConnectionValidator>())
                            .WithApplicationMessageInterceptor(serviceProvider.GetRequiredService<IMqttServerApplicationMessageInterceptor>());

                        if (!string.IsNullOrWhiteSpace(settings.IpAddress))
                            builder.WithDefaultEndpointBoundIPAddress(IPAddress.Parse(settings.IpAddress));

                        return builder;
                    })
                    .AddSingleton<IMqttServerOptions>(serviceProvider => serviceProvider.GetRequiredService<MqttServerOptionsBuilder>().Build())
                    .AddSingleton<IMqttServer>(serviceProvider => serviceProvider.GetRequiredService<IMqttFactory>().CreateMqttServer());

            services.AddSingleton<MqttHostedService>();

            return services;
        }
    }
}
