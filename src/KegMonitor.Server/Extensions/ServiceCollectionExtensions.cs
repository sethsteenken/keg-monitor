using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KegMonitor.Server
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKegMonitorServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthSettings>(configuration.GetSection("Auth"));

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
