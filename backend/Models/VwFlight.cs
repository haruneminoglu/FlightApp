namespace FlightApp.Models
{
    public class VwFlight
    {
        public int FlightId { get; set; }
        public string DepartureCity { get; set; } = null!;
        public string ArrivalCity { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime FlightDate { get; set; }
        public decimal Price { get; set; }
        public string AirlineName { get; set; } = null!;
    }
}
