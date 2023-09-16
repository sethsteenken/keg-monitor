using System.ComponentModel.DataAnnotations;

namespace KegMonitor.Web.Application
{
    public class ScaleAddModel
    {
        [Required(ErrorMessage = "Id is required.")]
        [Range(1, 1000, ErrorMessage = "Id must be greater than 0 and less than 1000.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Endpoint is required.")]
        public string Endpoint { get; set; }
    }
}
