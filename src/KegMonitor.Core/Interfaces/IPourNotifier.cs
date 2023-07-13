using KegMonitor.Core.Entities;

namespace KegMonitor.Core.Interfaces
{
    public interface IPourNotifier
    {
        Task NotifyAsync(Scale scale);
    }
}
