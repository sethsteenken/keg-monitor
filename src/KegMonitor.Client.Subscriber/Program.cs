// See https://aka.ms/new-console-template for more information
using KegMonitor.Client.Subscriber;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using System.Text;
using System.Text.Json;

// Creates a new client
var options = new MqttClientOptionsBuilder()
                    .WithClientId("KM_Web_Sub")
                    //.WithTcpServer("http://localhost", 5093)
                    //.WithTcpServer("192.168.1.164", 707)
                    .WithTcpServer("192.168.1.11", 707)
                    .WithCleanSession()
                    .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

var managedOptions = new ManagedMqttClientOptionsBuilder()
    .WithClientOptions(options)
    .Build();

try
{
    Console.WriteLine("Press any button to start MQTT subscriber client...");
    Console.ReadLine();

    // Creates the client object
    var client = new MqttFactory().CreateManagedMqttClient();

    client.ApplicationMessageReceivedAsync += msg =>
    {
        msg.ApplicationMessage.TryGetScaleNumber(out int scaleNumber);

        var payload = JsonSerializer.Deserialize<SensorPayload>(msg.ApplicationMessage.Payload);

        Console.WriteLine($"Scale {scaleNumber} | {payload.Time} | {payload.HX711.WeightRaw}");
        return Task.CompletedTask;
    };

    Console.WriteLine("Establishing connection...");
    await client.StartAsync(managedOptions);
    Console.WriteLine("Client connected.");

    Console.WriteLine("Subscribing...");
    await client.SubscribeAsync(new List<MqttTopicFilter>()
    {
        new MqttTopicFilter() { Topic = "tele/scale1/SENSOR" },
        new MqttTopicFilter() { Topic = "tele/scale2/SENSOR" },
    });
}
catch (Exception ex)
{
    Console.WriteLine($"** ERROR ** - {ex}");
}
finally
{
    Console.WriteLine("Press any key to quit...");
    Console.ReadLine();
}