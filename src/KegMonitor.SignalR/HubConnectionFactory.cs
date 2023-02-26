using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;

namespace KegMonitor.SignalR
{
    public class HubConnectionFactory : IAsyncDisposable
    {
        private ConcurrentDictionary<string, HubConnection> _connections 
            = new ConcurrentDictionary<string, HubConnection>();

        private readonly string _domain;

        public HubConnectionFactory(string domain)
        {
            _domain = domain;
        }

        public async Task<HubConnection> GetConnectionAsync(string endpoint)
        {
            if (_connections.TryGetValue(endpoint, out HubConnection? connection))
                return connection;

            connection = new HubConnectionBuilder()
                                .WithUrl(FormatUrl(endpoint))
                                .Build();

            await connection.StartAsync();

            _connections.TryAdd(endpoint, connection);

            return connection;
        }

        public virtual async ValueTask DisposeAsync()
        {
            foreach (var connection in _connections.Values)
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