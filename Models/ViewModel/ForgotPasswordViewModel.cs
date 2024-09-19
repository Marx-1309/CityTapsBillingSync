using System.ComponentModel.DataAnnotations;

namespace CityTapsBillingSync.Models.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
