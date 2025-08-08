using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.SignalR;
using KegMonitor.Web;
using KegMonitor.Web.Application;
using KegMonitor.Web.Authorization;
using KegMonitor.Web.Components;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

if (!bool.TryParse(builder.Configuration["RequireAuthentication"], out bool requireAuthentication))
    requireAuthentication = false; // default value

builder.Logging.Services.AddSignalRLogging();

builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

builder.Services.AddScoped<IAuthorizationHandler, AllowAnonymousAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EnvironmentDependentAuth", policyBuilder =>
    {
        if (requireAuthentication)
            policyBuilder.RequireAuthenticatedUser();
        else
            policyBuilder.AddRequirements(new AllowAnonymousAuthorizationRequirement());
    });
});

if (requireAuthentication) 
{ 
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApp(builder.Configuration);

    builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
    builder.Services.AddCascadingAuthenticationState();
}
else
{
    builder.Services.AddAuthentication();
}

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.SetConfigValues();
});

var healthCheckServices = builder.Services.AddHealthChecks()
                                          .AddCheck<SqlConnectionHealthCheck>("SQL Connection Health Check");

var mqttConfiguration = builder.Configuration.GetSection("Mqtt");

if (bool.TryParse(mqttConfiguration["Subscribe"], out bool subscribeToMqtt) && subscribeToMqtt)
    healthCheckServices.AddCheck<MqttConnectionHealthCheck>("MQTT Connection Health Check");

// added to support signalr client
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
});

builder.Services.AddKegMonitorDataAccess(builder.Configuration)
                .AddApplicationServices()
                .AddMqttClientServices(mqttConfiguration);

var app = builder.Build();

app.UseResponseCompression();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHealthChecks("/health")
   .RequireHost(app.Configuration["HealthCheckAllowedHosts"] ?? "*");

if (requireAuthentication)
    app.MapControllers();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapHub<ScaleHub>(ScaleHub.Endpoint);
app.MapHub<LogHub>(LogHub.Endpoint);

await app.InitializeDatabaseAsync();
await app.InitializeMqttAsync();

await app.RunAsync();