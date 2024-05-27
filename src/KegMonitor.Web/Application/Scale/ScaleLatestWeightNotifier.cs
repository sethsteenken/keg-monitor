using KegMonitor.Core.Entities;
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

        public async Task NotifyAsync(Scale scale, int weight)
        {
            if (scale == null)
                throw new ArgumentNullException(nameof(scale));

            await using var connection = await _hubConnectionFactory.CreateAndStartConnectionAsync(ScaleHub.Endpoint);
            await connection.SendAsync(nameof(ScaleHub.SendWeight), scale.Id, weight);
        }
    }
}
