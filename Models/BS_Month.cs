using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityTapsBillingSync.Models
{
    public class BS_Month
    {
        [Key]
        public long MonthID { get; set; }
        public string MonthName { get; set; }
    }
}
