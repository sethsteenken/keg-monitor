namespace KegMonitor.Core.Entities
{
    public class BeerPour : Entity
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private BeerPour() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public BeerPour(Beer beer, Scale scale, DateTime timeStamp) 
        {
            Beer = beer;
            Scale = scale;
            TimeStamp = timeStamp;
        }

        public Beer Beer { get; private set; }
        public Scale Scale { get; private set; }
        public DateTime TimeStamp { get; private set; }
    }
}
