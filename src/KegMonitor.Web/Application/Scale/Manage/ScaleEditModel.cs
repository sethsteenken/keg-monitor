using KegMonitor.Core.Entities;

namespace KegMonitor.Web.Application
{
    public class ScaleEditModel
    {
        public int Id { get; set; }
        public bool Active { get; set; }

        public int? BeerId { get; set; }

        public int CurrentWeight { get; set; }
        public int FullWeight { get; set; }
        public int EmptyWeight { get; set; }

        public int PourDifferenceThreshold { get; set; }

        public IEnumerable<Beer> BeerOptions { get; set; } = new List<Beer>();
    }
}
