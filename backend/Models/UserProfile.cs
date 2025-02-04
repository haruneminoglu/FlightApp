namespace FlightApp.Models
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string Firstname { get; set; }= string.Empty;
        public string Lastname { get; set; }= string.Empty;
        public string Email { get; set; }= string.Empty;
        public string MaskedPassword { get; set; }= string.Empty;
    }
}
