using Microsoft.AspNetCore.SignalR;

namespace KegMonitor.Web.Hubs
{
    public class ScaleHub : Hub
    {
        public const string Endpoint = "/scalehub";

        public const string ReceiveWeightPercentage = "ReceiveWeightPercentage";
        public const string ReceiveWeight = "ReceiveWeight";
        public const string ReceiveNewPour = "ReceiveNewPour";

        public async Task SendWeight(int scaleId, int weight)
        {
            await Clients.All.SendAsync(ReceiveWeight, scaleId, weight);
        }

        public async Task SendWeightPercentage(int scaleId, decimal percentage)
        {
            await Clients.All.SendAsync(ReceiveWeightPercentage, scaleId, percentage);
        }
        
        public async Task SendNewPour(int scaleId)
        {
            await Clients.All.SendAsync(ReceiveNewPour, scaleId, "Beer poured! Nice!");
        }
    }
}
