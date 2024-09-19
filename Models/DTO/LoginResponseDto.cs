using CityTapsBillingSync.Models.DTO;

namespace CityTapsBillingSync.Models
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
    }
}
