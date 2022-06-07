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

Console.WriteLine("Client started.");
Menu();

void Menu()
{
    try
    {
        Console.WriteLine(" - New Request -");
        Console.WriteLine("Enter Scale #:");
        var scaleIdValue = Console.ReadLine();
        if (!int.TryParse(scaleIdValue, out int scaleId))
        {
            Console.WriteLine("Scale # not a number. Try again.");
            Menu();
        }

        Console.WriteLine("Enter new Weight:");
        var weightValue = Console.ReadLine();
        if (!int.TryParse(weightValue, out int weight))
        {
            Console.WriteLine("New weight not a number. Try again.");
            Menu();
        }

        string topic = $"tele/scale{scaleId}/SENSOR";
        string json = JsonSerializer.Serialize(new { Time = DateTimeOffset.UtcNow, HX711 = new { WeightRaw = weight } });

        Console.WriteLine("Starting client...");
        _mqttClient.StartAsync(options).GetAwaiter().GetResult();

        Console.WriteLine($"Publishing message to {topic}...");
        _mqttClient.PublishAsync(topic, json).GetAwaiter().GetResult();
        Console.WriteLine("Publish complete.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"** ERROR ** - {ex}");
    }
    finally
    {
        Console.WriteLine("Stopping client...");
        _mqttClient.StopAsync().Wait();

        Console.WriteLine("Press any key to quit...");
        Console.ReadLine();
    }
}