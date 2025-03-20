using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KegMonitor.Web.Application
{
    public class SqlConnectionHealthCheck : IHealthCheck
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;

        public SqlConnectionHealthCheck(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var dbContext = _dbContextFactory.CreateDbContext();
                await dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}
