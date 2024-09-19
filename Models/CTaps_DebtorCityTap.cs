using System.ComponentModel.DataAnnotations;

namespace CityTapsBillingSync.Models
{
    public class CTaps_DebtorCityTap
    {
        [Key]
        public long DebtorCityTapID { get; set; }
        public string CUSTNMBR { get; set; }
        public string CUSTNMBR_CITYTAP { get; set; }
    }
}
