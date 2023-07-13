namespace KegMonitor.Web.Application
{
    public interface IMqttStartup
    {
        Task InitializeAsync();
    }
}
