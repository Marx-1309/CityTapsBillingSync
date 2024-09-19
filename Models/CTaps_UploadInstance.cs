using System.ComponentModel.DataAnnotations;

namespace CityTapsBillingSync.Models
{
    public class CTaps_UploadInstance
    {
        [Key]
        public long UploadInstanceID { get; set; }
        public BS_Month Month { get; set; }
        public long MonthId { get; set; }
        public long Year { get; set; }
        public DateTime? DateCreated { get; set; }
        public String Site { get; set; }
    }
}
