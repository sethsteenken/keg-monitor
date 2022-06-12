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
            services.AddSingleton<IScaleWeightHandler>(serviceProvider =>
            {
                return new ScaleWeightMessageHandler(
                    serviceProvider.GetRequiredService<IConfiguration>()["UIDomain"],
                    serviceProvider.GetRequiredService<ILogger<ScaleWeightMessageHandler>>());
            });

            services.AddSingleton<IMqttServerConnectionValidator, ConnectionValidator>()
                    .AddSingleton<IMqttServerApplicationMessageInterceptor, ApplicationMessageInterceptor>()
                    //.AddSingleton<MqttWebSocketServerAdapter>()
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
            //.AddSingleton<IMqttServer>(serviceProvider =>
            //{
            //    var adapters = new List<IMqttServerAdapter>
            //    {
            //        new MqttTcpServerAdapter(serviceProvider.GetRequiredService<IMqttNetLogger>())
            //        {
            //            TreatSocketOpeningErrorAsWarning = true
            //        },
            //        serviceProvider.GetRequiredService<MqttWebSocketServerAdapter>()
            //    };

            //    return serviceProvider.GetRequiredService<IMqttFactory>().CreateMqttServer(adapters);
            //});

            //services.AddSingleton<MqttConnectionHandler>();
            //services.AddSingleton<IMqttServerAdapter>(s => s.GetRequiredService<MqttConnectionHandler>());

            //services.AddSingleton<MqttHostedServer>();
            //services.AddSingleton<IHostedService>(s => s.GetRequiredService<MqttHostedServer>());
            //services.AddSingleton<MqttServer>(s => s.GetRequiredService<MqttHostedServer>());

            services.AddSingleton<MqttHostedService>();

            return services;
        }
    }
}
