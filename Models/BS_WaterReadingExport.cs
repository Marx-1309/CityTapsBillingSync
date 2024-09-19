using System.ComponentModel.DataAnnotations;

namespace CityTapsBillingSync.Models
{
    public class BS_WaterReadingExport
    {
        [Key]
        public long WaterReadingExportID { get; set; }
        public long MonthID { get; set; }
        public long Year { get; set; }
        public string SALSTERR { get; set; }
        public bool? LastReadings { get; set; }
    }
}
