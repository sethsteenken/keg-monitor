using KegMonitor.Core.Interfaces;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace KegMonitor.Web.Application
{
    public class ScaleWebPourNotifier : HubInteractor, IPourNotifier
    {
        public ScaleWebPourNotifier(HubUrlResolver urlResolver)
            : base (urlResolver, ScaleHub.Endpoint)
        {

        }

        public async Task NotifyAsync(int scaleId)
        {
            var connection = await GetHubConnectionAsync();
            await connection.SendAsync("SendNewPour", scaleId);
        }
    }
}
