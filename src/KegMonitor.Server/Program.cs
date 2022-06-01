using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.Server;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Server;

var configuration = new ConfigurationManager()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
                        .Build();

var serviceProvider = new ServiceCollection()
                        .AddSingleton<IConfiguration>(configuration)
                        .AddLogging(builder =>
                        {
                            builder.AddConfiguration(configuration.GetSection("Logging"));
                            builder.AddConsole();
                        })
                        .AddKegMonitorServices(configuration)
                        .BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true });

await using (var context = serviceProvider.GetRequiredService<IDbContextFactory<KegMonitorDbContext>>().CreateDbContext())
{
    await context.Database.MigrateAsync();
}

var options = serviceProvider.GetRequiredService<IMqttServerOptions>();

await serviceProvider.GetRequiredService<IMqttServer>().StartAsync(options);

// keep application running until user press a key
Console.ReadLine();
