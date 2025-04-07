using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KegMonitor.Infrastructure.EntityFramework
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKegMonitorDataAccess(
            this IServiceCollection services,
            string connectionString,
            bool useAzureManagedIdentity)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            services.AddDbContextFactory<KegMonitorDbContext>((serviceProvider, options) =>
            {
                if (useAzureManagedIdentity)
                {
                    //var tp = serviceProvider.GetService<ITokenProvider>();
                    //var token = tp.GetToken("https://ossrdbms-aad.database.windows.net");
                    //constr = constr.Replace("[passsword]", token);

                    // For user-assigned managed identity.
                    // var credential = new DefaultAzureCredential(
                    //     new DefaultAzureCredentialOptions
                    //     {
                    //         ManagedIdentityClientId = Environment.GetEnvironmentVariable("AZURE_MYSQL_CLIENTID");
                    //     });

                    var credential = new DefaultAzureCredential();

                    var tokenRequestContext = new TokenRequestContext(
                        ["https://ossrdbms-aad.database.windows.net/.default"]);
                    AccessToken accessToken = credential.GetToken(tokenRequestContext);
       
                    connectionString = $"{connectionString};Password={accessToken.Token}";
                }

                options.UseNpgsql(connectionString);
            });

            services.AddScoped<KegMonitorDbContext>(serviceProvider => serviceProvider.GetRequiredService<IDbContextFactory<KegMonitorDbContext>>().CreateDbContext());

            return services;
        }
    }
}
