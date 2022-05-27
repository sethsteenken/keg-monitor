using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KegMonitor.Server
{
    internal static class MqttApplicationMessageExtensions
    {
        public static bool IsSensorMessage(this MqttApplicationMessage message)
        {
            if (message == null)
                return false;

            return message.Topic.Contains("SENSOR");
        }

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
