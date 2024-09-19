using System;
using System.ComponentModel.DataAnnotations;

namespace CityTapsBillingSync.Models
{
    public class BS_WaterReadingExportData
    {
        [Key]
        //[Column("WaterReadingExportDataID")]
        public System.Int64? WaterReadingExportDataID { get; set; }
        public System.Int64 WaterReadingExportID { get; set; }
        public string? CUSTOMER_NUMBER { get; set; }
        public string? CUSTOMER_NAME { get; set; } = "";
        public string? AREA { get; set; }
        public string? ERF_NUMBER { get; set; }
        public string? METER_NUMBER { get; set; }
        public decimal? CURRENT_READING { get; set; }
        public decimal? PREVIOUS_READING { get; set; }
        public System.Int64? MonthID { get; set; }
        public System.Int64? Year { get; set; }
        public string? CUSTOMER_ZONING { get; set; }
        public System.Int64? RouteNumber { get; set; }
        public string? METER_READER { get; set; }
        public string? Comment { get; set; }
        public string? ReadingDate { get; set; }
        //public System.Int64? WaterReadingTypeID { get; set; }
        public byte[]? MeterImage { get; set; }
        public bool? IsCityTab { get; set; }                        
    }

}
