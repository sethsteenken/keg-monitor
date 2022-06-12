using KegMonitor.Broker;
using KegMonitor.SignalR;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSystemd();

builder.Logging.Services.AddSignalRLogging(builder.Configuration["Domain"]);
builder.Services.AddSignalR();
builder.Services.AddHostedService<MqttHostedService>();

//builder.Services.AddCors();
//builder.Services.AddConnections();
builder.Services.AddMqttServices(builder.Configuration);

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var app = builder.Build();

app.UseResponseCompression();
app.UseDeveloperExceptionPage();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

//app.UseCors(x => x.AllowAnyOrigin()
//                  .AllowAnyMethod()
//                  .AllowAnyHeader());

//if (bool.Parse(builder.Configuration["Host:WebSocketEnabled"]))
//{
//    app.UseMqttWebSocketEndpoint(path: builder.Configuration["Host:Path"]);
//}

//app.StartMqttServer();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<LogHub>(LogHub.Endpoint);
    //endpoints.MapConnectionHandler<MqttConnectionHandler>(builder.Configuration["Host:Path"], options =>
    //{
    //    options.WebSockets.SubProtocolSelector = MqttSubProtocolSelector.SelectSubProtocol;
    //});

    //endpoints.MapGet("/", async context =>
    //{
    //    var settings = context.RequestServices.GetRequiredService<IOptions<MqttServerSettings>>().Value;
    //    await context.Response.WriteAsync($"*** MQTT Broker - Service: {settings.ServiceName} ***");
    //});
});

// start?
_ = app.Services.GetRequiredService<MqttHostedService>();

await app.RunAsync();
