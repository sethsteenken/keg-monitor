﻿namespace KegMonitor.Core.Entities
{
    public class Scale : Entity
    {
        private Scale() { }

        public int CurrentWeight { get; private set; }
        public int FullWeight { get; set; }
        public int EmptyWeight { get; set; }

        public int RecordingDifferenceThreshold { get; set; }
        public int PourDifferenceThreshold { get; set; }
        public int MaxThreshold { get; set; }

        public bool Active { get; set; }

        public decimal Percentage => (decimal)Math.Round((decimal)(CurrentWeight - EmptyWeight) / (decimal)(FullWeight - EmptyWeight) * 100);

        public int Difference(int weight) => Math.Abs(CurrentWeight - weight);

        public Beer? Beer { get; set; }
        public IEnumerable<ScaleWeightChange> WeightChanges { get; private set; } = new List<ScaleWeightChange>();
        public DateTime LastUpdatedDate { get; set; }

        public void UpdateWeight(int weight)
        {
            if (CurrentWeight == weight)
                return;

            CurrentWeight = weight;
            LastUpdatedDate = DateTime.UtcNow;

            if (Beer != null)
                (WeightChanges as List<ScaleWeightChange>).Add(new ScaleWeightChange(this, Beer, weight, timestamp: LastUpdatedDate));
        }
    }
}
