namespace KegMonitor.Web.Application
{
    public interface ISensorManager
    {
        Task<SensorUpdateResult> BringOnlineAsync(int scaleId);
        Task<SensorUpdateResult> TakeOfflineAsync(int scaleId);
    }
}
