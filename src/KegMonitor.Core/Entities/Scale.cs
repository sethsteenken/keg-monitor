﻿namespace KegMonitor.Core.Entities
{
    public class Scale : Entity
    {
        private Scale() { }

        public int CurrentWeight { get; private set; }
        public int FullWeight { get; set; }
        public int EmptyWeight { get; set; }
        public int RecordingDifferenceThreshold { get; set; }

        public bool Active { get; set; }

        public decimal Percentage => (decimal)Math.Round((decimal)CurrentWeight / (decimal)FullWeight * 100);

        public Beer? Beer { get; private set; }
        public IEnumerable<ScaleWeightChange> WeightChanges { get; private set; } = new List<ScaleWeightChange>();
        public DateTime LastUpdatedDate { get; set; }

        public void UpdateWeight(int weight)
        {
            if (Beer is null)
                return;

            CurrentWeight = weight;
            (WeightChanges as List<ScaleWeightChange>).Add(new ScaleWeightChange(this, Beer, weight));
            LastUpdatedDate = DateTime.UtcNow;
        }
    }
}