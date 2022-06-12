using KegMonitor.Core.Interfaces;
using KegMonitor.SignalR;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace KegMonitor.Web.Application
{
    public class ScaleWebPourNotifier : IPourNotifier
    {
        private readonly HubConnectionFactory _hubConnectionFactory;

        public ScaleWebPourNotifier(HubConnectionFactory hubConnectionFactory)
        {
            _hubConnectionFactory = hubConnectionFactory;
        }

        public async Task NotifyAsync(int scaleId)
        {
            var connection = await _hubConnectionFactory.GetConnectionAsync(ScaleHub.Endpoint);
            await connection.SendAsync(nameof(ScaleHub.SendNewPour), scaleId);
        }
    }
}
