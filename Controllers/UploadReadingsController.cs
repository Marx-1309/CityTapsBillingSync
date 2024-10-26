using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CityTapsBillingSync.Models;
using CityTapsBillingSync.Data;
using CityTapsBillingSync.Services.IService;
using System.Text;
using ExcelDataReader;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace CityTapsBillingSync.Controllers
{
    [Authorize]
    public class UploadReadingsController : Controller
    {
        private readonly CityTapsBillingSyncContext _context;
        private readonly ICityTapsService _cityTapsService;
        public UploadReadingsController(CityTapsBillingSyncContext context, ICityTapsService cityTapsService)
        {
            _context = context;
            _cityTapsService = cityTapsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var months = await _context.BS_Month.ToListAsync();
                List<CTaps_UploadInstance> i = await _context.CTaps_UploadInstance.Include(m => m.Month).ToListAsync();

                var ii = await _context.CTaps_UploadInstance.Include(m => m.Month).OrderBy(r => r.MonthId).ToListAsync();
                var instanceMonthVm = new InstanceMonthVm
                {
                    uploadInstances = ii,
                    months = months
                };
                return View(instanceMonthVm);
            }
            catch(Exception ex)
            {
                return NotFound();
            }
            
        }

        [HttpPost]
        public async Task<ActionResult> Create(CTaps_UploadInstance uploadInstance, IFormFile FileUpload1)
        {

            //Task<int> result = new Task<int>(CountChars);
            //result.Start();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            try
            {
                CTaps_UploadInstance savedRecord = new CTaps_UploadInstance();

                var record = await _context.CTaps_UploadInstance.Where(r => r.Year == uploadInstance.Year && r.MonthId == uploadInstance.MonthId).FirstOrDefaultAsync();

                if (record != null)
                {
                    _context.RemoveRange(await _context.CTaps_Reading.Where(r => r.UploadInstanceId == record.UploadInstanceID).ToListAsync());
                    await _context.SaveChangesAsync();
                    //_context.RemoveRange(await _context.Pay_VIP.Where(r => r.UploadInstanceId == record.UploadInstanceID).ToListAsync());
                    //await _context.SaveChangesAsync();
                    _context.Remove(record);
                    await _context.SaveChangesAsync();

                };

                if (uploadInstance.MonthId != 0)
                {
                    // Add the new record to the context
                    var addedRecord = _context.CTaps_UploadInstance.Add(new CTaps_UploadInstance
                    {
                        MonthId = uploadInstance.MonthId,
                        Year = uploadInstance.Year,
                        DateCreated = uploadInstance.DateCreated,
                        Site = "OMARURU"
                    }).Entity;

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Return the added record with its data and ID populated
                    savedRecord = addedRecord;
                }

                if (FileUpload1 != null)
                {
                    UploadCTReadingsDoc(FileUpload1, savedRecord);
                }

            }
            catch (Exception ex)
            {

            }


            //// Add the uploadInstance to the context
            //_context.Pay_UploadInstance.Add(uploadInstance);
            //await _context.SaveChangesAsync();
            return Json("Successfully saved!"); // Redirect to the Index action after successful creation
        }

        public JsonResult UploadCTReadingsDoc(IFormFile FileUpload2, CTaps_UploadInstance savedRecord)
        {
            try
            {
                if (FileUpload2 != null)
                {
                    try
                    {
                        var fileName = FileUpload2.FileName; // Assuming file.FileName is a string
                        var fileExtension = Path.GetExtension(fileName);
                        var xlsExt = ".xls";
                        var xlsxExt = ".xlsx";
                        var uploadCount = 0;

                        if (fileExtension != xlsExt && fileExtension != xlsxExt)
                        {
                            ViewBag["error"] = "Please upload file with the correct extension ex. xlsx or xls!";
                            return Json("Please upload file with the correct extension ex.xlsx or xls!");
                        }

                        var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\CityTaps_Docs\\";

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, FileUpload2.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            FileUpload2.CopyToAsync(stream).GetAwaiter().GetResult();
                        }

                        {
                            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                List<CTaps_Reading> cTReadingItems = new List<CTaps_Reading>();

                                do
                                {

                                    bool isMyHeaderSkipped = false;

                                    //Incase you want to keep excel deductions data for one instance at a time 
                                    //var items = _context.Pay_Deduction.Where(c => c.EmployeeCode > 0).ToListAsync().GetAwaiter().GetResult();
                                    //if (items.Any())
                                    //{
                                    //    _context.RemoveRange(items);
                                    //    _context.SaveChangesAsync().GetAwaiter().GetResult();
                                    //}

                                    while (reader.Read())
                                    {
                                        if (!isMyHeaderSkipped)
                                        {
                                            isMyHeaderSkipped = true;
                                            continue;
                                        }
                                        if (Convert.ToString(reader.GetValue(0)) == "" && uploadCount > 0)
                                        {
                                            TempData["success"] = "Success Upload";
                                            //await Upload(FileUpload2, savedRecord);
                                            return Json("Successfully uploaded file1");

                                        }

                                        CTaps_Reading cTReadingItem = new CTaps_Reading();

                                        //if (_context.Pay_Deduction.Any(e => e.EmployeeCode == Convert.ToInt32(reader.GetValue(0))))
                                        //{

                                        //    ViewBag.Message = "A record with the same ID already exists";
                                        //    break; // Skip saving this record and move to the next one
                                        //}

                                        #region properties populate
                                        cTReadingItem.CustomerNo = reader.GetValue(0)?.ToString();
                                        if((reader.GetValue(1)?.ToString())=="null")
                                        {
                                            cTReadingItem.MeterSerial = "None";
                                        }
                                        else
                                        {
                                            cTReadingItem.MeterSerial = reader.GetValue(1)?.ToString();
                                        }

                                        object value = reader?.GetValue(2);
                                        if (value is null || value.ToString() == string.Empty || value.ToString()=="null" || value.ToString().Contains("-999"))
                                        {
                                            if(value is null || value.ToString() == string.Empty || value.ToString() == "null")
                                            {
                                                cTReadingItem.Reading = 0;
                                            }else if (value.ToString().Contains("-999"))
                                            {
                                                string r = value.ToString().Remove(0,4);
                                                
                                                cTReadingItem.Reading = (int?)decimal.Round(decimal.Parse(r));
                                                string i = "";
                                            }
                                            
                                        }
                                        else
                                        {
                                            cTReadingItem.Reading = Convert.ToInt32(value);
                                        }



                                        cTReadingItem.Timestamp = Convert.ToDateTime(reader.GetValue(3));
                                        //cTReadingItem.meterPointId = Convert.ToString(reader.GetValue(4));
                                        cTReadingItem.UploadInstanceId = (int)savedRecord.UploadInstanceID;
                                        #endregion

                                        _context.Add(cTReadingItem);
                                        _context.SaveChangesAsync().GetAwaiter().GetResult();
                                        cTReadingItems.Add(cTReadingItem);
                                        uploadCount++;
                                    }
                                    // Break after the loop if the condition was met at least once
                                }
                                while (reader.NextResult());
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            catch (Exception ex)
            {

            }

            return Json("File2 upload success");
        }

        [HttpGet]
        public JsonResult DeleteInstance(int? id)
        {
            if (id == null || _context.CTaps_UploadInstance.ToList() == null)
            {
                return Json("");
            }

            try
            {
                var pay_InstancePayroll = _context.CTaps_UploadInstance.FirstOrDefaultAsync(m => m.UploadInstanceID == id).GetAwaiter().GetResult();
                if (pay_InstancePayroll == null)
                {
                    return Json(false, $"Instance with an {id} not found!");
                }

                if (pay_InstancePayroll != null)
                {
                    _context.RemoveRange(_context.CTaps_Reading.Where(r => r.UploadInstanceId == pay_InstancePayroll.UploadInstanceID).ToListAsync().GetAwaiter().GetResult());
                    _context.SaveChangesAsync().GetAwaiter().GetResult();
                    _context.Remove(pay_InstancePayroll);
                    _context.SaveChangesAsync().GetAwaiter().GetResult();
                };
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false, $"Error : {ex.InnerException}");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetReadingsByUploadInstanceId(int InstanceId,string? status)
        {
            var instance = await _context.CTaps_UploadInstance.Where(r => r.UploadInstanceID == InstanceId).FirstOrDefaultAsync();
            if (instance != null)
            {
                ViewData["InstanceId"] = instance.UploadInstanceID;
                var insData = new
                {
                    MonthName = await _context.BS_Month.Where(r => r.MonthID == instance.MonthId).Select(r => r.MonthName.ToString().Trim()).FirstOrDefaultAsync(),
                    Year = instance.Year,
                    Site = instance.Site.Trim(),
                    DateCreated = instance.DateCreated
                };
                //var months = await _context.BS_Month.ToListAsync();

                var date = insData.DateCreated.ToString();

                char[] arrayStr = date.ToCharArray();

                var indx = Array.LastIndexOf(arrayStr, '/') + 5;
                TempData["instanceDetails"] =
                                                $" &nbsp;&nbsp;    Month : <strong> {insData.MonthName} </strong>" +
                                                $" &nbsp;&nbsp;    Year : <strong>{insData.Year}</strong>" +
                                                $" &nbsp;&nbsp;    Site : <strong>{insData.Site}</strong>" +
                                                $" &nbsp;&nbsp;    Date : <strong>{insData.DateCreated.ToString().Substring(0, indx)}</strong>";

                ViewBag.currentMonth = await _context.BS_Month.ToListAsync();

            }
            if(status == "all")
            {

                var previousInstId = _context.CTaps_Reading
                    .Where(x => x.UploadInstanceId < InstanceId)
                    .OrderByDescending(x => x.UploadInstanceId)
                    .Select(x => x.UploadInstanceId)
                    .FirstOrDefault();
                var customers = await _context.CTaps_Reading.Select(x => x.CustomerNo).Distinct().ToListAsync();

                var itemsAll = await _context.CTaps_Reading
                                        .Where(m => m.UploadInstanceId == previousInstId && customers
                                        .Contains(m.CustomerNo))
                                        .ToListAsync();
                var allReadings = _context.BS_WaterReadingExportData.ToList();

                foreach ( var item in itemsAll)
                {
                    
                }

                //return View(itemsAll);
            }
            var items = await _context.CTaps_Reading.Where(m => m.UploadInstanceId == InstanceId).ToListAsync();
            return View(items);

        }
    }
}
