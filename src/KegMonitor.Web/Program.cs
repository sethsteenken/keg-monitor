using KegMonitor.Core.Interfaces;
using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.Web.Application;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddScoped<IBeerQueryService, BeerQueryService>();
builder.Services.AddScoped<IBeerCommandService, BeerCommandService>();
builder.Services.AddScoped<IScaleQueryService, ScaleQueryService>();
builder.Services.AddScoped<IScaleCommandService, ScaleCommandService>();
builder.Services.AddScoped<IScaleDashboardQueryService, ScaleDashboardQueryService>();

builder.Services.AddSingleton<HubUrlResolver>();
builder.Services.AddSingleton<IScaleUpdater, ScaleWeightUpdater>();
builder.Services.AddScoped<IScaleWeightChangeNotifier, ScaleNewWeightPercentageNotifier>();
builder.Services.AddScoped<IScaleWeightChangeNotifier, ScaleLatestWeightNotifier>();
builder.Services.AddScoped<IPourNotifier, ScaleWebPourNotifier>();
builder.Services.AddScoped<IScaleWeightHandler, ScaleWeightHandler>();

var app = builder.Build();

app.UseResponseCompression();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapHub<ScaleHub>(ScaleHub.Endpoint);
app.MapFallbackToPage("/_Host");

app.MapPost("/scale/weight/", async delegate (HttpContext context)
{
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("ScaleWeightEndpointLogger");

    logger.LogDebug($"* New Request * - '/scale/weight/'");

    if (!int.TryParse(context.Request.Query["id"], out int scaleId))
        return;

    if (!int.TryParse(context.Request.Query["w"], out int weight))
        return;

    logger.LogDebug($" - ScaleId: {scaleId} | Weight: {weight}");

    await context.RequestServices.GetRequiredService<IScaleWeightHandler>().HandleAsync(scaleId, weight);
});

await using (var context = app.Services.GetRequiredService<IDbContextFactory<KegMonitorDbContext>>().CreateDbContext())
{
    await context.Database.MigrateAsync();
}

await app.RunAsync();
