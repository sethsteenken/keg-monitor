using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.SignalR;
using KegMonitor.Web;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// added to support signalr client
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
});

builder.Services.AddKegMonitorDataAccess(builder.Configuration)
                .AddApplicationServices()
                .AddMqttClientServices(builder.Configuration);

var app = builder.Build();

app.UseResponseCompression();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapDefaultControllerRoute();
app.MapHub<ScaleHub>(ScaleHub.Endpoint);
app.MapHub<LogHub>(LogHub.Endpoint);
app.MapFallbackToPage("/_Host");

await app.InitializeDatabaseAsync();
await app.InitializeMqttAsync();

await app.RunAsync();