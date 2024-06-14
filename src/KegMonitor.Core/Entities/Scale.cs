namespace KegMonitor.Core.Entities
{
    public class Scale : Entity
    {
        public const int DefaultUpdateInterval = 10;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Scale() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Scale(int id, string endpoint)
        {
            Id = id;
            Endpoint = endpoint;
            
            // default topic to Tasmoto naming based on Id
            Topic = $"tele/scale{id}/SENSOR";

            Active = false;
            LastUpdatedDate = DateTime.UtcNow;
        }

        public int CurrentWeight { get; private set; }
        public int FullWeight { get; set; }
        public int EmptyWeight { get; set; }

        public int PourDifferenceThreshold { get; set; }

        public bool Active { get; set; }

        public string Topic { get; set; }
        public string Endpoint { get; set; }

        public decimal Percentage => CalculatePercentage(CurrentWeight);

        public Beer? Beer { get; set; }
        public IEnumerable<ScaleWeightChange> WeightChanges { get; private set; } = new List<ScaleWeightChange>();
        public DateTime LastUpdatedDate { get; set; }

        public DateTime? LastEnabledDate { get; set; }
        public DateTime? LastDisabledDate { get; set; }

        public bool SensorOnline
        {
            get
            {
                if (WeightChanges == null || !WeightChanges.Any())
                    return false;

                var latestTimeStamp = WeightChanges.OrderByDescending(wc => wc.TimeStamp)
                                                   .Select(w => w.TimeStamp)
                                                   .First();

                return latestTimeStamp >= DateTime.UtcNow.AddSeconds(-DefaultUpdateInterval)
                    && (LastDisabledDate is null || LastDisabledDate.Value < latestTimeStamp); }
        }

        public decimal CalculatePercentage(int weight)
        {
            var weightDiff = FullWeight - EmptyWeight;
            if (weightDiff == 0)
                return 0;

            return (decimal)Math.Round((decimal)(weight - EmptyWeight) / (decimal)weightDiff * 100, 2);
        }

        public bool IsPour(int weight, DateTime timeStamp)
        {
            if (CurrentWeight == weight)
                return false;

            // no pour if scale recently enabled
            if (LastEnabledDate != null && LastEnabledDate.Value > timeStamp.AddSeconds(-DefaultUpdateInterval))
                return false;

            // no pour if very recent pour occurred - allow for some time to pass
            if (Beer != null && Beer.Pours.Any() && Beer.Pours.OrderByDescending(p => p.TimeStamp).First().TimeStamp > timeStamp.AddSeconds(-(DefaultUpdateInterval + 2)))
                return false;

            var difference = Math.Abs(CurrentWeight - weight);
            return Active && Beer != null && difference > PourDifferenceThreshold;
        }

        public void ForceSetCurrentWeight(int weight)
        {
            if (Active)
                return;

            CurrentWeight = weight;
        }

        public ScaleUpdateResult UpdateWeight(int weight)
        {
            var timeStamp = DateTime.UtcNow;
            bool isPourEvent = IsPour(weight, timeStamp);

            (WeightChanges as List<ScaleWeightChange>).Add(
                new ScaleWeightChange(this, weight, timeStamp, Beer, isPourEvent));

            if (Active)
            {
                CurrentWeight = weight;
                LastUpdatedDate = timeStamp;

                if (Beer != null && isPourEvent)
                    Beer.AddPour(this, timeStamp);
            }

            return new ScaleUpdateResult(PourOccurred: isPourEvent);
        }
    }
}
