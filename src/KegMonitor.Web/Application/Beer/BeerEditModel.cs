using System.ComponentModel.DataAnnotations;

namespace KegMonitor.Web.Application
{
    public class BeerEditModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name must be {1} characters or less.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [MaxLength(200, ErrorMessage = "Type must be {1} characters or less.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "ABV is required.")]
        public decimal ABV { get; set; }

        public decimal OG { get; set; }
        public decimal FG { get; set; }

        public decimal CalculatedABV
        {
            get
            {
                if (OG <= 0 || FG <= 0) 
                    return 0;

                return (OG - FG) * 131.25M;
            }
        }

        public DateTime? TapDate { get; set; }
        public DateTime? BrewDate { get; set; }
        public DateTime? SecondaryDate { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public int? IBU { get; set; }
        public string Ingredients { get; set; }
        public string Recipe { get; set; }

        [Url(ErrorMessage = "Url must be a valid URL.")]
        public string Url { get; set; }

        public string Notes { get; set; }
    }
}
