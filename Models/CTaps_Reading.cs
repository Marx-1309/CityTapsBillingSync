using AutoMapper.Configuration.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CityTapsBillingSync.Models
{
    public class CTaps_Reading
    {
        [Key]
        public long? CTReadingId { get; set; }

        [DisplayName("Meter Piont")]
        [Column("MeterPointId")]
        [NotMapped]
        [JsonIgnore]
        public string? MeterPointId {  get; set; }

        [DisplayName("Meter Serial")]
        public string? MeterSerial { get; set; }

        [DisplayName("Customer No")]
        public string? CustomerNo { get; set; }

        [DisplayName("Active")]
        public bool? IsMeterActive { get; set; }

        public DateTime? Timestamp { get; set; }
        [DisplayName("CurrentReading")]
        public int? Reading { get; set; }
        [NotMapped]
        [Ignore]
        [DisplayName("PreviousReading")]
        public int? PreviousReading { get; set; }
        public int UploadInstanceId {  get; set; }
    }
}
