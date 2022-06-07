using KegMonitor.Core.Interfaces;
using KegMonitor.Core.Services;
using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.Web.Application;
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

app.MapPost("/scale/pour/{scaleId}/", delegate (HttpContext context)
{
    string scaleId = context.Request.RouteValues["scaleId"].ToString();
    Console.Write($"**** New Request **** - ScaleId: {scaleId}");
});

await app.RunAsync();
