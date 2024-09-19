using CityTapsBillingSync.Models;

namespace CityTapsBillingSync.Models
{
    public class InstanceMonthVm
    {
        public List<BS_Month> months { get; set; } 
        public List<CTaps_UploadInstance> uploadInstances { get; set; } 
    }
}
