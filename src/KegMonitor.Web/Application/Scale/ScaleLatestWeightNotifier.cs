using KegMonitor.Core.Interfaces;
using KegMonitor.SignalR;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace KegMonitor.Web.Application
{
    public class ScaleLatestWeightNotifier : IScaleWeightChangeNotifier
    {
        private readonly HubConnectionFactory _hubConnectionFactory;

        public ScaleLatestWeightNotifier(
            HubConnectionFactory hubConnectionFactory)
        {
            _hubConnectionFactory = hubConnectionFactory;
        }

        public async Task NotifyAsync(int scaleId, int weight)
        {
            var connection = await _hubConnectionFactory.GetConnectionAsync(ScaleHub.Endpoint);
            await connection.SendAsync(nameof(ScaleHub.SendWeight), scaleId, weight);
        }
    }
}
