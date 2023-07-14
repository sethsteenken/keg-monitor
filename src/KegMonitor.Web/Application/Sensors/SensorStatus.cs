using MudBlazor;

namespace KegMonitor.Web.Application
{
    public class SensorStatus
    {
        public SensorStatus(SensorStatusOption status)
        {
            Status = status;

            switch (status)
            {
                case SensorStatusOption.Unknown:
                    Text = "Checking sensor...";
                    Color = Color.Default;
                    break;
                case SensorStatusOption.Online:
                    Text = "Sensor online.";
                    Color = Color.Success;
                    break;
                case SensorStatusOption.Offline:
                    Text = "Sensor offline";
                    Color = Color.Error;
                    break;
                default:
                    break;
            }
        }

        public SensorStatusOption Status { get; private set; }
        public string Text { get; private set; }
        public Color Color { get; private set; }
    }
}
