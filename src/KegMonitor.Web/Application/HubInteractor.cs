using Microsoft.AspNetCore.SignalR.Client;

namespace KegMonitor.Web.Application
{
    public abstract class HubInteractor : IAsyncDisposable
    {
        private readonly HubUrlResolver _urlResolver;
        private readonly string _endpoint;

        private HubConnection _hubConnection;

        public HubInteractor(HubUrlResolver urlResolver, string endpoint)
        {
            _urlResolver = urlResolver;
            _endpoint = endpoint;
        }

        protected async Task<HubConnection> GetHubConnectionAsync() 
        { 
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                                        .WithUrl(_urlResolver.Resolve(_endpoint))
                                        .Build();

                await _hubConnection.StartAsync();
            }

            return _hubConnection;
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
                await _hubConnection.DisposeAsync();
        }
    }
}
