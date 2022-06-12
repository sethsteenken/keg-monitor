// See https://aka.ms/new-console-template for more information
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System.Text.Json;

// Creates a new client
MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId("Tester9")
                                        //.WithTcpServer("http://localhost", 5093)
                                        .WithTcpServer("192.168.1.20", 707);
                                        //.WithTcpServer("192.168.1.11", 707);
                                        //.WithWebSocketServer("192.168.1.11:707")
                                        //.WithProxy("http://localhost:5003")
                                        //.WithCommunicationTimeout(TimeSpan.FromSeconds(10));

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
_mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(context =>
{
    Console.WriteLine("Failed to connect.");
    Console.WriteLine(context.Exception.ToString());
});

try
{
    Console.WriteLine("Press any button to start MQTT client...");
    Console.ReadLine();

    Console.WriteLine("Starting client...");
    await _mqttClient.StartAsync(options);
    Console.WriteLine("Client started.");

    await MenuAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"** ERROR ** - {ex}");
}
finally
{
    Console.WriteLine("Stopping client...");
    await _mqttClient.StopAsync();

    Console.WriteLine("Press any key to quit...");
    Console.ReadLine();
}


async Task MenuAsync()
{
    Console.WriteLine(" - New Request -");
    Console.WriteLine("Enter Scale #:");
    var scaleIdValue = Console.ReadLine();
    if (!int.TryParse(scaleIdValue, out int scaleId))
    {
        Console.WriteLine("Scale # not a number. Try again.");
        await MenuAsync();
    }

    Console.WriteLine("Enter new Weight:");
    var weightValue = Console.ReadLine();
    if (!int.TryParse(weightValue, out int weight))
    {
        Console.WriteLine("New weight not a number. Try again.");
        await MenuAsync();
    }

    string topic = $"tele/scale{scaleId}/SENSOR";
    string json = JsonSerializer.Serialize(new { Time = DateTimeOffset.UtcNow, HX711 = new { WeightRaw = weight } });

    Console.WriteLine($"Publishing message to {topic}...");
    await _mqttClient.PublishAsync(topic, json);
    Console.WriteLine("Publish complete.");

    Console.WriteLine("Continue? [y/n]");
    var continueAnswer = Console.ReadLine();
    if (continueAnswer == "y")
        await MenuAsync();
}