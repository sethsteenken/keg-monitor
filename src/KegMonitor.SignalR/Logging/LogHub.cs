using Microsoft.AspNetCore.SignalR;

namespace KegMonitor.SignalR
{
    public class LogHub : Hub
    {
        public const string Endpoint = "/loghub";

        public const string ReceiveMessage = "ReceiveMessage";

        public async Task SendMessage(string logger, string level, string message)
        {
            await Clients.All.SendAsync(ReceiveMessage, logger, level, message);
        }
    }
}
