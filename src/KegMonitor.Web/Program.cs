using KegMonitor.Core;
using KegMonitor.Core.Interfaces;
using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.SignalR;
using KegMonitor.Web;
using KegMonitor.Web.Application;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using MudBlazor;
using MudBlazor.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false);

builder.Logging.Services.AddSignalRLogging();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

// added to support signalr client
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddKegMonitorDataAccess(builder.Configuration);

builder.Services.AddScoped<IFileUploader>(serviceProvider =>
{
    return new FileUploader(serviceProvider.GetRequiredService<IWebHostEnvironment>(), "uploads");
});

builder.Services.AddScoped<IBeerQueryService, BeerQueryService>();
builder.Services.AddScoped<IBeerCommandService, BeerCommandService>();
builder.Services.AddScoped<IScaleQueryService, ScaleQueryService>();
builder.Services.AddScoped<IScaleCommandService, ScaleCommandService>();
builder.Services.AddScoped<IScaleDisplayQueryService, ScaleDisplayQueryService>();
builder.Services.AddScoped<IScaleDashboardQueryService, ScaleDashboardQueryService>();

builder.Services.AddScoped<HubConnectionFactory>(serviceProvider =>
{
    return new HubConnectionFactory(serviceProvider.GetRequiredService<IConfiguration>()["WebDomain"]);
});

builder.Services.AddSingleton<IScaleUpdater, ScaleWeightUpdater>();
builder.Services.AddScoped<IScaleWeightChangeNotifier, ScaleNewWeightPercentageNotifier>();
builder.Services.AddScoped<IScaleWeightChangeNotifier, ScaleLatestWeightNotifier>();
builder.Services.AddScoped<IPourNotifier, ScaleWebPourNotifier>();
builder.Services.AddScoped<IScaleWeightHandler, ScaleWeightHandler>();
builder.Services.AddSingleton<IHealthChecker, HealthChecker>();

builder.Services.AddMqttClientServices(builder.Configuration);

var app = builder.Build();

app.UseResponseCompression();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapHub<ScaleHub>(ScaleHub.Endpoint);
app.MapHub<LogHub>(LogHub.Endpoint);
app.MapFallbackToPage("/_Host");

app.MapPost("/log/", async delegate (HttpContext context)
{
    if (!context.Request.HasJsonContentType())
    {
        context.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
        return;
    }

    var logMessage = await context.Request.ReadFromJsonAsync<LogMessage>();

    context.RequestServices.GetRequiredService<ILoggerFactory>()
                           .CreateLogger(logMessage.Logger)
                           .Log(logMessage);

    context.Response.StatusCode = (int)HttpStatusCode.Accepted;
});

app.MapGet("/health/", async delegate (HttpContext context)
{
    var healthy = await context.RequestServices.GetRequiredService<IHealthChecker>().CheckAsync(context.RequestAborted);
    context.Response.StatusCode = healthy ? (int)HttpStatusCode.Accepted : (int)HttpStatusCode.InternalServerError;
});

if (bool.TryParse(app.Configuration["MigrateDatabaseToLatest"], out bool migrate) && migrate)
{
    await using (var context = app.Services.GetRequiredService<IDbContextFactory<KegMonitorDbContext>>().CreateDbContext())
    {
        await context.Database.MigrateAsync();
    }
}

if (bool.TryParse(app.Configuration["Mqtt:Subscribe"], out bool subscribe) && subscribe)
{
    // subscribe to mqtt broker
    var mqttClientOptions = app.Services.GetRequiredService<ManagedMqttClientOptions>();
    var mqttClient = app.Services.GetRequiredService<IManagedMqttClient>();
    await mqttClient.StartAsync(mqttClientOptions);

    // TODO - use configuration to establish filter connections

    await mqttClient.SubscribeAsync(new List<MqttTopicFilter>()
    {
        new MqttTopicFilter() { Topic = "tele/scale1/SENSOR" },
        new MqttTopicFilter() { Topic = "tele/scale2/SENSOR" },
    });
}

await app.RunAsync();