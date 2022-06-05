using KegMonitor.Core.Entities;

namespace KegMonitor.Web.Application
{
    public interface IBeerQueryService
    {
        Task<BeerEditModel> BuildEditModelAsync(int? id);
        Task<IEnumerable<Beer>> GetAllAsync();
    }
}
