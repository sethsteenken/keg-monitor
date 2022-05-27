namespace KegMonitor.Core.Entities
{
    public class Beer : Entity
    {
        public Beer(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public decimal ABV { get; set; }

        public DateTime LastUpdatedDate { get; set; }
    }
}
