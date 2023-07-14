namespace KegMonitor.Web.Application
{
    public class ScaleSensor : IAsyncDisposable
    {
        public ScaleSensor(SensorStatus status = null)
        {
            Status = status ?? new SensorStatus(SensorStatusOption.Unknown);
        }

        public SensorStatus Status { get; private set; }
        public Timer Timer { get; private set; }
        protected bool ManuallySetStatus { get; private set; }

        public event EventHandler OnChange;

        public async Task InitializeAsync()
        {
            switch (Status.Status)
            {
                case SensorStatusOption.Unknown:
                    await ResetMonitoringTimerAsync();
                    break;
                case SensorStatusOption.Online:
                    await SetAsOnlineAsync();
                    break;
                case SensorStatusOption.Offline:
                    await SetAsOfflineAsync();
                    break;
            };

            InvokeChange();
        }

        public Task ResetMonitoringTimerAsync()
        {
            const int dueTime = 12000;
            const int period = 12000;

            if (Timer != null)
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                Timer.Change(dueTime, period);
            }
            else
            {
                Timer = new Timer(async (state) =>
                {
                    await SetAsOfflineAsync();
                }, null, dueTime, period);
            }

            return Task.CompletedTask;
        }

        public async Task SetAsOnlineAsync()
        {
            Status = new SensorStatus(SensorStatusOption.Online);
            await ResetMonitoringTimerAsync();

            InvokeChange();
        }

        public Task SetAsOfflineAsync()
        {
            Status = new SensorStatus(SensorStatusOption.Offline);
            InvokeChange();

            return Task.CompletedTask;
        }

        protected void InvokeChange()
        {
            OnChange?.Invoke(this, new EventArgs());

        }

        public async ValueTask DisposeAsync()
        {
            if (Timer != null)
                await Timer.DisposeAsync();
        }
    }
}
