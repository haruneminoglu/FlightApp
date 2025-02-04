namespace FlightApp.Models
{
    public class FlightClass
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }=string.Empty;
        public decimal PriceMultiplier { get; set; }
    }
}
