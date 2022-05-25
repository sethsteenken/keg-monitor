// See https://aka.ms/new-console-template for more information
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System.Text.Json;

Console.WriteLine("Hello, World!");


// Creates a new client
MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId("Dev.To")
                                        .WithTcpServer("localhost", 707);
                                        //.WithTcpServer("192.168.1.20", 707);

// Create client options objects
ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                        .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                        .WithClientOptions(builder.Build())
                        .Build();

// Creates the client object
IManagedMqttClient _mqttClient = new MqttFactory().CreateManagedMqttClient();

// Set up handlers
_mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(context => Console.WriteLine("Successfully connected."));
_mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(context => Console.WriteLine("Successfully disconnected."));
_mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(context => Console.WriteLine("Failed to connect."));

// Starts a connection with the Broker
_mqttClient.StartAsync(options).GetAwaiter().GetResult();

// Send a new message to the broker every second
while (true)
{
    string json = JsonSerializer.Serialize(new { message = "Heyo :)", sent = DateTimeOffset.UtcNow });
    _mqttClient.PublishAsync("dev.to/topic/json", json);

    Task.Delay(1000).GetAwaiter().GetResult();
}