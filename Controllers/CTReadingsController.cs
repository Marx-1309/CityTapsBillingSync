using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CityTapsBillingSync.Data;
using CityTapsBillingSync.Models;
using CityTapsBillingSync.Models.DTO;
using Newtonsoft.Json;
using CityTapsBillingSync.Services.IService;
using IdentityModel;
using CityTapsBillingSync.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Security;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace CityTapsBillingSync.Controllers
{
    [Authorize]
    public class CTReadingsController : Controller
    {
        private readonly CityTapsBillingSyncContext _context;
        private readonly ICityTapsService _cityTapsService;
        public CTReadingsController(CityTapsBillingSyncContext context, ICityTapsService cityTapsService)
        {
            _context = context;
            _cityTapsService = cityTapsService;
        }
        
        [HttpGet]
        public async Task<IActionResult> CityTapReadingsIndex()
        {
            List<CTaps_Reading>? list = new();
            BS_WaterReadingExport LatestExport = await _context.BS_WaterReadingExport.OrderBy(r=>r.WaterReadingExportID).Where(r=>r.SALSTERR.Contains("OMARURU")).LastOrDefaultAsync();

             var str = await _context.BS_Month.Where(r => r.MonthID == LatestExport.MonthID).FirstOrDefaultAsync();
            
            ViewBag.latestYear = LatestExport.Year;
            ViewBag.latestMonth = str.MonthName.Trim();
            ViewBag.currentMonth = _context.BS_Month.ToList();
            

            ResponseDTO? response = await _cityTapsService.GetCityTapsReadingsAsync();

            if (response != null && response.IsSuccess)
            {
                list =  (List<CTaps_Reading>)response.Result;
                ViewBag.readingsCount = list.Count;
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ManageAverageList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageNegativeList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageZeroList()
        {
            return View();
        }

        public async Task<JsonResult> ImportCityTapsReadingsAsync(BS_WaterReadingExport waterReadingExport)
        {
            string[] logedInUser = User.Identity.Name.Trim().ToString().Split('@');

            
            BS_WaterReadingExport ReadingExport = new();

            try
            {
                if(!(waterReadingExport.Year ==0 && waterReadingExport.MonthID == 0))
                {
                    ReadingExport = await _context.BS_WaterReadingExport.Where(r => r.MonthID == waterReadingExport.MonthID && r.Year == waterReadingExport.Year).FirstOrDefaultAsync();
                    if(ReadingExport is null)
                    {
                       // BS_Month monthObj = await _context.BS_Month.Where(r => r.MonthID == ReadingExport.MonthID).FirstOrDefaultAsync();



                        return Json(false,$"Reading export with {waterReadingExport.Year}  was not found");
                        
                    }
                }
                else
                {
                    ReadingExport = await _context.BS_WaterReadingExport.OrderByDescending(r=>r.WaterReadingExportID).LastOrDefaultAsync();
                }

                var machingExport = await _context.CTaps_UploadInstance.Where(r => r.MonthId == waterReadingExport.MonthID && r.Year == waterReadingExport.Year ).FirstOrDefaultAsync();

                List<CTaps_Reading>? list = new();
                int syncCount = 0;

                var response = await _context.CTaps_Reading.Where(r=>r.UploadInstanceId == machingExport.UploadInstanceID).ToListAsync();

                if (response != null /*&& response.Count>0*/)
                {
                    list = (List<CTaps_Reading>)response;
                    var validList = list.Where(r => r.CustomerNo.Contains("OM")).ToList();
                    foreach (var item in validList)
                    {
                        var cityTapsCust = await  _context.BS_DebtorCityTap.Where(r => r.CUSTNMBR_CITYTAP.Trim() == item.CustomerNo.Trim()).FirstOrDefaultAsync();
                        if(cityTapsCust is null)
                        {
                            continue;
                        }
                        cityTapsCust.CUSTNMBR.Trim();
                        cityTapsCust.CUSTNMBR_CITYTAP.Trim();
                        var billingCust =await _context.BS_WaterReadingExportData.Where(r => r.CUSTOMER_NUMBER == cityTapsCust.CUSTNMBR &&
                                                                      r.WaterReadingExportID == ReadingExport.WaterReadingExportID)
                                                                     .FirstOrDefaultAsync();
                        if(billingCust is null)
                        {
                            continue;
                        }
                        else
                        {
                            billingCust.CURRENT_READING = item.Reading;
                            billingCust.IsCityTab = true;

                            if (logedInUser.Length > 0)
                            {
                                billingCust.METER_READER = logedInUser[0];
                            }
                            else
                            {
                                billingCust.METER_READER = "Unknown";
                            }
                            billingCust.ReadingDate = DateTime.UtcNow.ToString();

                            _context.BS_WaterReadingExportData.Update(billingCust);
                            _context.SaveChanges();
                        }
                        syncCount++;
                    }
                }
                else
                {
                    TempData["error"] = "Something went wrong!";
                }

                return Json(true,$"{syncCount} readings updated.");
            }
            catch (Exception ex)
            {
                return Json(false,$"{ex.Message.ToString()}");
            }
        }

        //public async Task<JsonResult> ImportCityTapsReadingsAsync(BS_WaterReadingExport waterReadingExport)
        //{

        //    BS_WaterReadingExport ReadingExport = new();

        //    try
        //    {
        //        if (!(waterReadingExport.Year == 0 && waterReadingExport.MonthID == 0))
        //        {
        //            ReadingExport = await _context.BS_WaterReadingExport.Where(r => r.MonthID == waterReadingExport.MonthID && r.Year == waterReadingExport.Year).FirstOrDefaultAsync();
        //            if (ReadingExport is null)
        //            {
        //                // BS_Month monthObj = await _context.BS_Month.Where(r => r.MonthID == ReadingExport.MonthID).FirstOrDefaultAsync();



        //                return Json(false, $"Reading export with {waterReadingExport.Year}  was not found");

        //            }
        //        }
        //        else
        //        {
        //            ReadingExport = await _context.BS_WaterReadingExport.OrderByDescending(r => r.WaterReadingExportID).LastOrDefaultAsync();
        //        }



        //        List<CTReading>? list = new();
        //        int syncCount = 0;

        //        ResponseDTO? response = await _cityTapsService.GetCityTapsReadingsAsync();

        //        if (response != null && response.IsSuccess)
        //        {
        //            list = (List<CTReading>)response.Result;
        //            var validList = list.Where(r => r.CustomerNo.Contains("OM")).ToList();
        //            foreach (var item in validList)
        //            {
        //                var cityTapsCust = await _context.BS_DebtorCityTap.Where(r => r.CUSTNMBR_CITYTAP.Trim() == item.CustomerNo.Trim()).FirstOrDefaultAsync();
        //                cityTapsCust.CUSTNMBR.Trim();
        //                cityTapsCust.CUSTNMBR_CITYTAP.Trim();
        //                var billingCust = await _context.BS_WaterReadingExportData.Where(r => r.CUSTOMER_NUMBER == cityTapsCust.CUSTNMBR &&
        //                                                              r.WaterReadingExportID == ReadingExport.WaterReadingExportID)
        //                                                             .FirstOrDefaultAsync();
        //                if (billingCust is null)
        //                {
        //                    _context.BS_WaterReadingExportData.Add(new BS_WaterReadingExportData { });
        //                }
        //                else
        //                {
        //                    billingCust.CURRENT_READING = item.Reading;
        //                    billingCust.IsCityTab = true;
        //                    billingCust.METER_READER = "UserX";
        //                    billingCust.ReadingDate = DateTime.UtcNow.ToString();

        //                    _context.BS_WaterReadingExportData.Update(billingCust);
        //                    _context.SaveChanges();
        //                }
        //                syncCount++;
        //            }
        //        }
        //        else
        //        {
        //            TempData["error"] = response?.Message;
        //        }

        //        return Json(true, $"{syncCount} readings updated.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(false, $"{ex.Message.ToString()}");
        //    }
        //}
    }
}
