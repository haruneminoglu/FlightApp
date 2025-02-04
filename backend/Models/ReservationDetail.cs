public class ReservationDetail
{
    public int ReservationId { get; set; }
    public int UserId { get; set; }
    public int PassengerId { get; set; }
    public int FlightId { get; set; }
    public string ClassName { get; set; }=string.Empty;
    public DateTime ReservationDate { get; set; }
    public string DepartureCity { get; set; }=string.Empty;
    public string ArrivalCity { get; set; }=string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public DateTime FlightDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string AirlineName { get; set; }=string.Empty;
}
