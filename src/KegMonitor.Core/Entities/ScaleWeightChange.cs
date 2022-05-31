namespace KegMonitor.Core.Entities
{
    public class ScaleWeightChange : Entity
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ScaleWeightChange() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
