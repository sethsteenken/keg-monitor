using KegMonitor.Core.Entities;
using KegMonitor.Core.Interfaces;
using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.SignalR;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class ScaleNewWeightPercentageNotifier : IScaleWeightChangeNotifier
    {
        private readonly HubConnectionFactory _hubConnectionFactory;
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;

        public ScaleNewWeightPercentageNotifier(
            HubConnectionFactory hubConnectionFactory,
            IDbContextFactory<KegMonitorDbContext> dbContextFactory)
        {
            _hubConnectionFactory = hubConnectionFactory;
            _dbContextFactory = dbContextFactory;
        }

        public async Task NotifyAsync(Scale scale, int weight)
        {
            if (scale == null)
                throw new ArgumentNullException(nameof(scale));
            
            var connection = await _hubConnectionFactory.GetConnectionAsync(ScaleHub.Endpoint);
            await connection.SendAsync(nameof(ScaleHub.SendWeightPercentage), scale.Id, scale.Percentage);
        }
    }
}
