using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KegMonitor.Infrastructure.EntityFramework
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKegMonitorDataAccess(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContextFactory<KegMonitorDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<KegMonitorDbContext>(serviceProvider => serviceProvider.GetRequiredService<IDbContextFactory<KegMonitorDbContext>>().CreateDbContext());

            return services;
        }
    }
}
