using KegMonitor.Core.Entities;
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

        public async Task NotifyAsync(Scale scale)
        {
            if (scale == null)
                throw new ArgumentNullException(nameof(scale));

            var connection = await _hubConnectionFactory.GetConnectionAsync(ScaleHub.Endpoint);
            await connection.SendAsync(nameof(ScaleHub.SendNewPour), scale.Id);
        }
    }
}
