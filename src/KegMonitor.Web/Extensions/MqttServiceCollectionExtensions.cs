using KegMonitor.Core.Interfaces;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using System.Text.Json;

namespace KegMonitor.Web
{
    internal static class MqttServiceCollectionExtensions
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

            return services;
        }
    }
}
