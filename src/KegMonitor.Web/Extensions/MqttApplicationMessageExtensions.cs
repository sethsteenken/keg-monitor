using MQTTnet;

namespace KegMonitor.Web
{
    internal static class MqttApplicationMessageExtensions
    {
        public static bool TryGetScaleNumber(this MqttApplicationMessage message, out int scaleNumber)
        { 
            scaleNumber = 0;

            if (message == null || string.IsNullOrWhiteSpace(message.Topic))
                return false;

            var topicArgs = message.Topic.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (topicArgs.Length == 3 && topicArgs[0] == "tele" && topicArgs[2] == "SENSOR" && int.TryParse(topicArgs[1].Replace("scale", ""), out scaleNumber))
                return true;

            return false;
        }
    }
}
