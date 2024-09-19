using CityTapsBillingSync.Models.DTO;

namespace CityTapsBillingSync.Services.IService
{
    public interface ICityTapsService
    {
         Task<ResponseDTO> GetCityTapsReadingsAsync();

    }
}
