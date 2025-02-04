namespace FlightApp.Models
{
    public class AdminPassengerView
    {
        public int PassengerId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MaskedTcNo { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerSurname { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}