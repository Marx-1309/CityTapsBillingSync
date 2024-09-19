using CityTapsBillingSync.Models.DTO;
using CityTapsBillingSync.Services.IService;
using CityTapsBillingSync.Utility;

namespace CityTapsBillingSync.Services
{
    public class CityTapService : ICityTapsService
    {
        private readonly BaseService _baseService;
        public CityTapService(BaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> GetCityTapsReadingsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = CityTapsBillingSync.Utility.SD.ApiType.GET,
                Url = "https://server.watermeter.dev.perspectiv.in/api/v1/meterReading/getbulkreading"
            });
        }
    }
}
