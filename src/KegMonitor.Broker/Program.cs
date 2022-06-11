using KegMonitor.Broker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMqttServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseMqttWebSocketEndpoint("/mqtt");
app.StartMqttServer();
app.Run();
