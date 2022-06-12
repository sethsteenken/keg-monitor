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

        public async Task NotifyAsync(int scaleId, int weight)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var scale = await context.Scales.FirstOrDefaultAsync(s => s.Id == scaleId);
            if (scale == null)
                throw new InvalidOperationException("Scale not found.");

            var connection = await _hubConnectionFactory.GetConnectionAsync(ScaleHub.Endpoint);
            await connection.SendAsync(nameof(ScaleHub.SendWeightPercentage), scaleId, scale.Percentage);
        }
    }
}
