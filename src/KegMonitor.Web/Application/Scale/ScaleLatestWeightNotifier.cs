using KegMonitor.Core.Interfaces;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace KegMonitor.Web.Application
{
    public class ScaleLatestWeightNotifier : HubInteractor, IScaleWeightChangeNotifier
    {
        public ScaleLatestWeightNotifier(HubUrlResolver urlResolver)
            : base (urlResolver, ScaleHub.Endpoint)
        {

        }

        public async Task NotifyAsync(int scaleId, int weight)
        {
            var connection = await GetHubConnectionAsync();
            await connection.SendAsync(nameof(ScaleHub.SendWeight), scaleId, weight);
        }
    }
}
