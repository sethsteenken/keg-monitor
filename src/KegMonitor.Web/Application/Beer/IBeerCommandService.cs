namespace KegMonitor.Web.Application
{
    public interface IBeerCommandService
    {
        Task<int> SaveAsync(BeerEditModel model);
        Task DeleteAsync(int id);
        Task RemoveImageAsync(int id);
    }
}
