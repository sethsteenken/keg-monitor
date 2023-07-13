using KegMonitor.Core.Entities;

namespace KegMonitor.Core.Interfaces
{
    public interface IScaleWeightChangeNotifier
    {
        Task NotifyAsync(Scale scale, int weight);
    }
}
