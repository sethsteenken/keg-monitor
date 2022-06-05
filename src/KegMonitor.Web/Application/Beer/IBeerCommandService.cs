namespace KegMonitor.Web.Application
{
    public interface IBeerCommandService
    {
        Task<int> SaveAsync(BeerEditModel model);
    }
}
