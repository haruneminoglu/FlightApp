using System.ComponentModel.DataAnnotations;

namespace FlightApp.Models
{
    public class Airline
    {
        [Key]
        public int AirlineId { get; set; }

        [Required]
        [MaxLength(100)]
        public string AirlineName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string AirlineCountry { get; set; } = string.Empty;
    }
}
