namespace KegMonitor.Web.Application
{
    public interface IBeerQueryService
    {
        Task<BeerEditModel> BuildEditModelAsync(int? id);
    }
}
