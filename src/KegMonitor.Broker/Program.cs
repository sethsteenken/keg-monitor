using KegMonitor.Broker;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSystemd();
builder.Services.AddHostedService<MqttHostedService>();

//builder.Services.AddCors();
//builder.Services.AddConnections();
builder.Services.AddMqttServices(builder.Configuration);

var app = builder.Build();

app.UseDeveloperExceptionPage();
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
    //endpoints.MapConnectionHandler<MqttConnectionHandler>(builder.Configuration["Host:Path"], options =>
    //{
    //    options.WebSockets.SubProtocolSelector = MqttSubProtocolSelector.SelectSubProtocol;
    //});

    endpoints.MapGet("/", async context =>
    {
        var settings = context.RequestServices.GetRequiredService<IOptions<MqttServerSettings>>().Value;
        await context.Response.WriteAsync($"*** MQTT Broker - Service: {settings.ServiceName} ***");
    });
});

// start?
_ = app.Services.GetRequiredService<MqttHostedService>();

await app.RunAsync();
