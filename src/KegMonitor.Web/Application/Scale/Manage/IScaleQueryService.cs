namespace KegMonitor.Web.Application
{
    public interface IScaleQueryService
    { 
        Task<ScaleEditModel> BuildEditModelAsync(int id);
    }
}
