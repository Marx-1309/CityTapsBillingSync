using System.ComponentModel.DataAnnotations;

namespace CityTapsBillingSync.Models.ViewModel
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
