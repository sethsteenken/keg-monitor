﻿namespace KegMonitor.Core.Entities
{
    public class Beer : Entity
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Beer() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Beer(string? name, string? type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public decimal ABV { get; set; }
        public decimal OG { get; set; }
        public decimal FG { get; set; }
        public int? IBU { get; set; }
        public string? ImagePath { get; set; }
        public string? Ingredients { get; set; }
        public string? Recipe { get; set; }
        public string? Url { get; set; }
        public string? Notes { get; set; }

        public DateTime? BrewDate { get; set; }
        public DateTime? SecondaryDate { get; set; }
        public DateTime? TapDate { get; set; }

        public string FullName => $"{Name}{TapDate?.ToString(" (yyyy)")}";

        public string ImagePathOrDefault => string.IsNullOrWhiteSpace(ImagePath) ? "/img/beer_default.jpg" : ImagePath;

        public DateTime LastUpdatedDate { get; set; }

        public IEnumerable<BeerPour> Pours { get; private set; } = new List<BeerPour>();

        public void AddPour(Scale scale, DateTime timeStamp)
        {
            (Pours as List<BeerPour>).Add(new BeerPour(this, scale, timeStamp));
        }
    }
}
