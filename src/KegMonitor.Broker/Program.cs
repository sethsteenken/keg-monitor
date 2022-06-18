using KegMonitor.Broker;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false);

builder.Host.UseSystemd();

builder.Logging.Services.AddSingleton<ILoggerProvider>(serviceProvider =>
{
    string domain = serviceProvider.GetRequiredService<IConfiguration>()["WebDomain"];
    return new WebRequestLoggerProvider($"{domain.TrimEnd('/')}/log/");
});

builder.Services.AddHostedService<MqttHostedService>();
builder.Services.AddMqttServices(builder.Configuration);

var app = builder.Build();

_ = app.Services.GetRequiredService<MqttHostedService>();

await app.RunAsync();
