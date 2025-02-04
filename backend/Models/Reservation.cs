public class Reservation
{
    public int ReservationId { get; set; }
    public int UserId { get; set; }
    public int FlightId { get; set; }
    public int PassengerId { get; set; }
    public int ClassId { get; set; } 
    public DateTime ReservationDate { get; set; }
}
