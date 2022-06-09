namespace KegMonitor.Web.Application
{
    public class ScaleDisplayItem
    {
        public int Id { get; set; }
        public int Weight { get; set; }
        public double Percentage { get; set; }
        public bool Active { get; set; }
        public BeerDisplayItem Beer { get; set; }
    }
}
