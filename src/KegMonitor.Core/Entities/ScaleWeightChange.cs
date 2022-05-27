namespace KegMonitor.Core.Entities
{
    public class ScaleWeightChange : Entity
    {
        public ScaleWeightChange(Scale scale, int weight)
        {
            Scale = scale;
            Weight = weight;
            TimeStamp = DateTime.Now;
        }

        public Scale Scale { get; private set; }  
        public int Weight { get; private set; }
        public DateTime TimeStamp { get; private set; }
    }
}
