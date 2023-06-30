using KegMonitor.Core.Interfaces;
using KegMonitor.SignalR;
using KegMonitor.Web.Application;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using System.Text.Json;

namespace KegMonitor.Web
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MqttBrokerSettings>(configuration.GetSection("Mqtt"));
            services.AddSingleton<ManagedMqttClientOptions>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<MqttBrokerSettings>>().Value;
                var options = new MqttClientOptionsBuilder()
                                .WithClientId(settings.ClientId)
                                .WithTcpServer(settings.IpAddress, settings.Port)
                                .WithCredentials(settings.Username, settings.Password)
                                .WithCleanSession()
                                .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                                .Build();

                return new ManagedMqttClientOptionsBuilder()
                    .WithClientOptions(options)
                    .Build();
            });

            services.AddSingleton<IManagedMqttClient>(serviceProvider =>
            {
                var client = new MqttFactory().CreateManagedMqttClient();

                client.ApplicationMessageReceivedAsync += async msg =>
                {
                    msg.ApplicationMessage.TryGetScaleNumber(out int scaleNumber);

                    var payload = JsonSerializer.Deserialize<SensorPayload>(msg.ApplicationMessage.Payload);

                    await using (var scope = serviceProvider.CreateAsyncScope())
                    {
                        await scope.ServiceProvider.GetRequiredService<IScaleWeightHandler>()
                                    .HandleAsync(scaleNumber, payload.HX711.WeightRaw);
                    }    
                };

                return client;
            });

            services.AddSingleton<IMqttStartup, MqttStartup>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IFileUploader>(serviceProvider =>
            {
                return new FileUploader(serviceProvider.GetRequiredService<IWebHostEnvironment>(), "uploads");
            });

            services.AddScoped<IBeerQueryService, BeerQueryService>();
            services.AddScoped<IBeerCommandService, BeerCommandService>();
            services.AddScoped<IScaleQueryService, ScaleQueryService>();
            services.AddScoped<IScaleCommandService, ScaleCommandService>();
            services.AddScoped<IScaleDisplayQueryService, ScaleDisplayQueryService>();
            services.AddScoped<IScaleDashboardQueryService, ScaleDashboardQueryService>();

            services.AddScoped<HubConnectionFactory>(serviceProvider =>
            {
                return new HubConnectionFactory(serviceProvider.GetRequiredService<IConfiguration>()["WebDomain"]);
            });

            services.AddSingleton<IScaleUpdater, ScaleWeightUpdater>();
            services.AddScoped<IScaleWeightChangeNotifier, ScaleNewWeightPercentageNotifier>();
            services.AddScoped<IScaleWeightChangeNotifier, ScaleLatestWeightNotifier>();
            services.AddScoped<IPourNotifier, ScaleWebPourNotifier>();
            services.AddScoped<IScaleWeightHandler, ScaleWeightHandler>();
            services.AddSingleton<IHealthChecker, HealthChecker>();

            return services;
        }
    }
}
