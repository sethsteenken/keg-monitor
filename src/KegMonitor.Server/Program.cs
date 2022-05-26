using KegMonitor.Server;
using MQTTnet.Server;

var configuration = new ConfigurationBuilder().BuildConfiguration();

var serviceProvider = new ServiceCollection()
                        .AddSingleton<IConfiguration>(configuration)
                        .AddLogging(builder =>
                        {
                            builder.AddConsole();
                        })
                        .AddKegMonitorServices()
                        .BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true });

var options = serviceProvider.GetRequiredService<IMqttServerOptions>();

await serviceProvider.GetRequiredService<IMqttServer>().StartAsync(options);

// keep application running until user press a key
Console.ReadLine();
