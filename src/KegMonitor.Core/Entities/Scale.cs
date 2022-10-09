namespace KegMonitor.Core.Entities
{
    public class Scale : Entity
    {
        private Scale() { }

        public int CurrentWeight { get; private set; }
        public int FullWeight { get; set; }
        public int EmptyWeight { get; set; }

        public int PourDifferenceThreshold { get; set; }

        public bool Active { get; set; }

        public decimal Percentage => CalculatePercentage(CurrentWeight);

        public Beer? Beer { get; set; }
        public IEnumerable<ScaleWeightChange> WeightChanges { get; private set; } = new List<ScaleWeightChange>();
        public DateTime LastUpdatedDate { get; set; }

        public decimal CalculatePercentage(int weight) => (decimal)Math.Round((decimal)(weight - EmptyWeight) / (decimal)(FullWeight - EmptyWeight) * 100, 2);

        public ScaleUpdateResult UpdateWeight(int weight, bool recordChangeEvent = true, bool checkForPour = true)
        {
            if (CurrentWeight == weight)
                return new ScaleUpdateResult();

            var timeStamp = DateTime.UtcNow;
            var difference = Math.Abs(CurrentWeight - weight);

            bool isPourEvent = checkForPour && Active 
                && Beer != null && difference > PourDifferenceThreshold;

            if (Active)
            {
                CurrentWeight = weight;
                LastUpdatedDate = timeStamp;
            }

            if (recordChangeEvent)
            {
                (WeightChanges as List<ScaleWeightChange>).Add(
                    new ScaleWeightChange(this, weight, timestamp: timeStamp, beer: Beer, isPourEvent: isPourEvent));
            }

            return new ScaleUpdateResult(Recorded: true, PourOccurred: isPourEvent);
        }
    }
}
