namespace KegMonitor.Web.Application
{
    public interface IScaleCommandService
    {
        Task UpdateActiveStateAsync(int scaleId, bool active);
        Task<int> SaveAsync(ScaleEditModel model);
        Task DeleteAsync(int id);
    }
}
