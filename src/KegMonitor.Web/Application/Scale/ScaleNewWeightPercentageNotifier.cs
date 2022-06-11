using KegMonitor.Core.Interfaces;
using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.Web.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class ScaleNewWeightPercentageNotifier : HubInteractor, IScaleWeightChangeNotifier
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;

        public ScaleNewWeightPercentageNotifier(
            HubUrlResolver urlResolver,
            IDbContextFactory<KegMonitorDbContext> dbContextFactory)
            : base (urlResolver, ScaleHub.Endpoint)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task NotifyAsync(int scaleId, int weight)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var scale = await context.Scales.FirstOrDefaultAsync(s => s.Id == scaleId);
            if (scale == null)
                throw new InvalidOperationException("Scale not found.");

            var connection = await GetHubConnectionAsync();
            await connection.SendAsync(nameof(ScaleHub.SendWeightPercentage), scaleId, scale.Percentage);
        }
    }
}
