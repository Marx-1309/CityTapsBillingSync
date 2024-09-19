using CityTapsBillingSync.Models;
using CityTapsBillingSync.Models.DTO;

namespace CityTapsBillingSync.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDTO?> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto);
    }
}
