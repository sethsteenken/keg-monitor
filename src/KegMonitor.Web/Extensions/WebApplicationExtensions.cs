using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.Web.Application;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web
{
    public static class WebApplicationExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            if (bool.TryParse(app.Configuration["MigrateDatabaseToLatest"], out bool migrate) && migrate)
            {
                await using var context = app.Services.GetRequiredService<IDbContextFactory<KegMonitorDbContext>>().CreateDbContext();
                await context.Database.MigrateAsync();
            }
        }

        public static Task InitializeMqttAsync(this WebApplication app)
        {
            return app.Services.GetRequiredService<IMqttStartup>().InitializeAsync();
        }
    }
}
