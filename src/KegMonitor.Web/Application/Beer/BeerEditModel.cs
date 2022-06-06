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

        public string Description { get; set; }
    }
}
