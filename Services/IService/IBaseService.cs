using CityTapsBillingSync.Models.DTO;

namespace CityTapsBillingSync.Services.IService
{
    public interface IBaseService
    {
        Task<ResponseDTO?> SendAsync(RequestDTO requestDto, bool withBearer = true);
    }
}
