using MudBlazor;

namespace KegMonitor.Web.Application
{
    public class ScaleSensor : IAsyncDisposable
    {
        public ScaleSensor()
        {
            StatusColor = Color.Default;
            StatusText = "Checking sensor...";
        }

        public bool Online { get; private set; }
        public Color StatusColor { get; private set; }
        public string StatusText { get; private set; }
        public Timer Timer { get; private set; }

        public Task ResetMonitoringTimerAsync()
        {
            const int dueTime = 15000;
            const int period = 15000;

            if (Timer != null)
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                Timer.Change(dueTime, period);
            }
            else
            {
                Timer = new Timer(async (state) =>
                {
                    await TakeOfflineAsync();
                }, null, dueTime, period);
            }

            return Task.CompletedTask;
        }

        public async Task BringOnlineAsync()
        {
            Online = true;
            StatusColor = Color.Success;
            StatusText = "Sensor online.";
            await ResetMonitoringTimerAsync();
        }

        public Task TakeOfflineAsync()
        {
            Online = false;
            StatusColor = Color.Error;
            StatusText = "Sensor offline.";

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (Timer != null)
                await Timer.DisposeAsync();
        }
    }
}
