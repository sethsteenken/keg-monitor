using MudBlazor;

namespace KegMonitor.Web.Application
{
    public class ScaleDisplayItem
    {
        public int Id { get; set; }
        public int Weight { get; set; }
        public double Percentage { get; set; }
        public bool Active { get; set; }
        public BeerDisplayItem Beer { get; set; }

        public Color Color
        {
            get
            {
                if (Percentage <= 10)
                    return Color.Error;
                else if (Percentage <= 25)
                    return Color.Warning;
                else
                    return Color.Success;
            }
        }
    }
}
