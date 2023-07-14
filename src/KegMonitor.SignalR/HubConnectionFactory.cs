using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;

namespace KegMonitor.SignalR
{
    public class HubConnectionFactory : IAsyncDisposable
    {
        private ConcurrentBag<HubConnection> _connections 
            = new ConcurrentBag<HubConnection>();

        private readonly string _domain;

        public HubConnectionFactory(string domain)
        {
            _domain = domain;
        }

        public async Task<HubConnection> GetConnectionAsync(string endpoint)
        {
            var connection = new HubConnectionBuilder()
                                .WithUrl(FormatUrl(endpoint))
                                .Build();

            await connection.StartAsync();

            _connections.Add(connection);

            return connection;
        }

        public virtual async ValueTask DisposeAsync()
        {
            foreach (var connection in _connections)
            {
                if (connection is not null)
                    await connection.DisposeAsync();
            }

            _connections.Clear();
        }

        public string FormatUrl(string endpoint)
        {
            return $"{_domain.TrimEnd('/')}/{endpoint?.TrimStart('/')}";
        }
    }
}