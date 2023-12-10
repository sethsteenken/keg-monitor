using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;

namespace KegMonitor.Web
{
    public static class MqttClientExtensions
    {
        public static async Task RefreshTopicSubscriptionAsync(this IManagedMqttClient mqttClient, string oldTopic, string newTopic)
        {
            if (string.IsNullOrEmpty(oldTopic))
                throw new ArgumentNullException(nameof(oldTopic));
            if (string.IsNullOrEmpty(newTopic))
                throw new ArgumentNullException(nameof(newTopic));

            await mqttClient.UnsubscribeAsync(oldTopic);
            await mqttClient.SubscribeAsync(newTopic);
        }
    }
}
