using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.Extensions;
using MQTTnet.Server;
using System.Net;
using System.Text;

int messageCounter = 0;

IMqttServer mqttServer = new MqttFactory().CreateMqttServer();

var options = new MqttServerOptionsBuilder();

options.WithDefaultEndpointBoundIPAddress(IPAddress.Parse("192.168.1.20"))
       .WithDefaultEndpointPort(707)
       .WithConnectionValidator(context =>
       {
           Console.WriteLine($"New connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");
       })
       .WithApplicationMessageInterceptor(context =>
       {
           var payload = context.ApplicationMessage?.Payload == null ?
               null : Encoding.UTF8.GetString(context.ApplicationMessage.Payload);

           messageCounter++;

           Console.WriteLine($"MessageId: {messageCounter} - TimeStamp: {DateTime.Now} -- Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage?.Topic}, Payload = {payload}, QoS = {context.ApplicationMessage?.QualityOfServiceLevel}, Retain-Flag = {context.ApplicationMessage?.Retain}");
       });

// start the server with options  
await mqttServer.StartAsync(options.Build());

// keep application running until user press a key
Console.ReadLine();
