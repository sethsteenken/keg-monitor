using KegMonitor.Core.Interfaces;

namespace KegMonitor.Web.Application
{
    public class ScaleLatestWeightNotifier : IPourNotifier
    {
        public ScaleLatestWeightNotifier()
        {

        }

        public Task NotifyAsync(int scaleId)
        {
            return Task.CompletedTask;
        }
    }
}
