using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightApp.Models
{
    public class Flight
    {
        [Key]
        public int FlightId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Kalkış Şehri")]
        public string DepartureCity { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Varış Şehri")]
        public string ArrivalCity { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Kalkış Zamanı")]
        public DateTime DepartureTime { get; set; }

        [Required]
        [Display(Name = "Varış Zamanı")]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [Display(Name = "Uçuş Tarihi")]
        public DateTime FlightDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        [ForeignKey("Airline")]
        [Required]
        [Display(Name = "Havayolu")]
        public int AirlineId { get; set; }
        public Airline? Airline { get; set; }
    }
}
