using Microsoft.AspNetCore.SignalR.Client;

namespace KegMonitor.SignalR
{
    public class HubConnectionFactory
    {
        private readonly string _domain;

        public HubConnectionFactory(string domain)
        {
            _domain = domain;
        }

        public HubConnection CreateConnection(string endpoint)
        {
            return new HubConnectionBuilder()
                                .WithUrl(FormatUrl(endpoint))
                                .Build();
        }

        public async Task<HubConnection> CreateAndStartConnectionAsync(string endpoint)
        {
            var connection = CreateConnection(endpoint);
            await connection.StartAsync();
            return connection;
        }

        private string FormatUrl(string endpoint)
        {
            return $"{_domain.TrimEnd('/')}/{endpoint?.TrimStart('/')}";
        }
    }
}