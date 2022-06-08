using KegMonitor.Core.Interfaces;
using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.Web.Application;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using System.Text;

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
builder.Services.AddKegMonitorDataAccess(builder.Configuration);

builder.Services.AddScoped<IBeerQueryService, BeerQueryService>();
builder.Services.AddScoped<IBeerCommandService, BeerCommandService>();
builder.Services.AddScoped<IScaleQueryService, ScaleQueryService>();
builder.Services.AddScoped<IScaleCommandService, ScaleCommandService>();

builder.Services.AddSingleton<IScaleWeightHandler, ScaleWeightHandler>();
builder.Services.AddSingleton<IPourNotifier, ScaleLatestWeightNotifier>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapPost("/scale/weight/", async delegate (HttpContext context)
{
    Console.Write($"* New Request * - '/scale/weight/'");

    if (!int.TryParse(context.Request.Query["id"], out int scaleId))
        return;

    if (!int.TryParse(context.Request.Query["w"], out int weight))
        return;

    Console.Write($" - ScaleId: {scaleId} | Weight: {weight}");

    await context.RequestServices.GetRequiredService<IScaleWeightHandler>().HandleAsync(scaleId, weight);
});

await using (var context = app.Services.GetRequiredService<IDbContextFactory<KegMonitorDbContext>>().CreateDbContext())
{
    await context.Database.MigrateAsync();
}

await app.RunAsync();
