using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.SignalR;
using KegMonitor.Web;
using KegMonitor.Web.Application;
using KegMonitor.Web.Components;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Net;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.Services.AddSignalRLogging();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    builder.Configuration.Bind(Constants.AzureAd, options);
                    // TODO - remove this line when token validation issue is fixed.
                    //options.TokenValidationParameters.ValidateIssuer = false;
                });

builder.Services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.SetConfigValues();
});

builder.Services.AddHealthChecks()
                .AddCheck<SqlConnectionHealthCheck>("SQL Connection Health Check")
                .AddCheck<MqttConnectionHealthCheck>("MQTT Connection Health Check");

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

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHealthChecks("/health")
    .RequireHost("localhost");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<ScaleHub>(ScaleHub.Endpoint);
app.MapHub<LogHub>(LogHub.Endpoint);

await app.InitializeDatabaseAsync();
await app.InitializeMqttAsync();

await app.RunAsync();