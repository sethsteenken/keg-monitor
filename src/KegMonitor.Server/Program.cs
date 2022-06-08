using KegMonitor.Core.Interfaces;
using KegMonitor.Server;
using MQTTnet;
using MQTTnet.Server;
using System.Net;

var configuration = new ConfigurationManager()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
                        .Build();

var services = new ServiceCollection()
                        .AddSingleton<IConfiguration>(configuration)
                        .AddLogging(builder =>
                        {
                            builder.AddConfiguration(configuration.GetSection("Logging"));
                            builder.AddConsole();
                        });

services.Configure<AuthSettings>(configuration.GetSection("Auth"));
services.AddSingleton<IScaleWeightHandler>(serviceProvider =>
{
    return new ScaleWeightMessageHandler(
        serviceProvider.GetRequiredService<IConfiguration>()["WebUI:Domain"],
        serviceProvider.GetRequiredService<ILogger<ScaleWeightMessageHandler>>());
});

services.AddSingleton<IMqttServerConnectionValidator, ConnectionValidator>()
        .AddSingleton<IMqttServerApplicationMessageInterceptor, ApplicationMessageInterceptor>()
        .AddSingleton<IMqttServer>(serviceProvider => new MqttFactory().CreateMqttServer())
        .AddSingleton<MqttServerOptionsBuilder>(serviceProvider =>
        {
            return new MqttServerOptionsBuilder()
                .WithDefaultEndpointBoundIPAddress(IPAddress.Parse(configuration["Host:IpAddress"]))
                .WithDefaultEndpointPort(int.Parse(configuration["Host:Port"]))
                .WithConnectionValidator(serviceProvider.GetRequiredService<IMqttServerConnectionValidator>())
                .WithApplicationMessageInterceptor(serviceProvider.GetRequiredService<IMqttServerApplicationMessageInterceptor>());
        })
        .AddSingleton<IMqttServerOptions>(serviceProvider => serviceProvider.GetRequiredService<MqttServerOptionsBuilder>().Build());

var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true });

var options = serviceProvider.GetRequiredService<IMqttServerOptions>();

Console.WriteLine("Starting MQTT server...");
await serviceProvider.GetRequiredService<IMqttServer>().StartAsync(options);
Console.WriteLine("MQTT server started.");

// keep application running until user press a key
Console.ReadLine();
