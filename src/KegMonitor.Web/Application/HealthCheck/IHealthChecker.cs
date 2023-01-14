namespace KegMonitor.Web.Application
{
    public interface IHealthChecker
    {
        Task<bool> CheckAsync(CancellationToken cancellationToken = default);
    }
}
