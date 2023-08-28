using KegMonitor.Core.Entities;

namespace KegMonitor.Web.Application
{
    public interface IScaleQueryService
    { 
        Task<ScaleEditModel> BuildEditModelAsync(int id);
        Task<Scale> GetWithPoursAsync(int id);
    }
}
