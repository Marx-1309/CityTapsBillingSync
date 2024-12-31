namespace CityTapsBillingSync.Models
{
    public class CustomerReadingToExcel
    {
        public string CustomerNumber { get; set; }
        public string CityTapCustomerNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public string CustomerName { get; set; }
        public string ErfNumber { get; set; }
        public string MeterNumber { get; set; }
        public string Area { get; set; }
        public decimal? PreviousReading { get; set; }
        public decimal? CurrentReading { get; set; }
    }
}
