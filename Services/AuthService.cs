//using CityTapsBillingSync.Models;
//using CityTapsBillingSync.Models.DTO;
//using CityTapsBillingSync.Service.IService;
//using CityTapsBillingSync.Services.IService;
//using CityTapsBillingSync.Utility;

//namespace Mango.Web.Service
//{
//    public class AuthService : IAuthService
//    {
//        private readonly IBaseService _baseService;
//        public AuthService(IBaseService baseService)
//        {
//            _baseService = baseService;
//        }

//        public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
//        {
//            return await _baseService.SendAsync(new RequestDTO()
//            {
//                ApiType = SD.ApiType.POST,
//                Data = registrationRequestDto,
//                Url = SD.AuthAPIBase + "/api/auth/AssignRole"
//            });
//        }

//        public async Task<ResponseDTO?> LoginAsync(LoginRequestDto loginRequestDto)
//        {
//            return await _baseService.SendAsync(new RequestDTO()
//            {
//                ApiType = SD.ApiType.POST,
//                Data = loginRequestDto,
//                Url = SD.AuthAPIBase + "/api/auth/login"
//            }, withBearer: false);
//        }

//        public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
//        {
//            return await _baseService.SendAsync(new RequestDTO()
//            {
//                ApiType = SD.ApiType.POST,
//                Data = registrationRequestDto,
//                Url = SD.AuthAPIBase + "/api/auth/register"
//            }, withBearer: false);
//        }
//    }
//}
