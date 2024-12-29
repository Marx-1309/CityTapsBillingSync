using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CityTapsBillingSync.Models;
using CityTapsBillingSync.Data;
using CityTapsBillingSync.Services.IService;
using System.Text;
using ExcelDataReader;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using NuGet.Packaging.Licenses;

namespace CityTapsBillingSync.Controllers
{
    [Authorize]
    [Controller]
    public class UploadReadingsController : Controller
    {
        private readonly CityTapsBillingSyncContext _context;
        private readonly ICityTapsService _cityTapsService;
        string SqlConnectionString;
        private IConfiguration _configuration;
        string sqlQuery = $"SELECT DISTINCT \r\n    WRED.CUSTOMER_NUMBER,\r\n    CT.CUSTNMBR_CITYTAP as CITY_TAP_CUSTOMER_NUMBER,\r\n    [Timestamp],\r\n    WRED.CUSTOMER_NAME,\r\n    WRED.ERF_NUMBER,\r\n    WRED.METER_NUMBER,\r\n    WRED.AREA,\r\n    WRED.PREVIOUS_READING,\r\n    WRED.CURRENT_READING\r\n  \r\nFROM \r\n    BS_WaterReadingExportData WRED\r\n   JOIN CTaps_Reading CTR ON WRED.CUSTOMER_NUMBER = (SELECT  CUSTNMBR FROM BS_DebtorCityTap WHERE CUSTNMBR_CITYTAP = CTR.CustomerNo)\r\n   JOIN \r\n    BS_DebtorCityTap CT \r\n    ON WRED.CUSTOMER_NUMBER = CT.CUSTNMBR \r\nWHERE  \r\n    WRED.IsCityTab = 1 \r\n    AND WRED.WaterReadingExportID = 6\r\n    AND WRED.CURRENT_READING < WRED.PREVIOUS_READING;";
        public UploadReadingsController(CityTapsBillingSyncContext context, ICityTapsService cityTapsService, IConfiguration configuration)
        {
            _context = context;
            _cityTapsService = cityTapsService;
            _configuration = configuration;
            SqlConnectionString = @"Data Source=localhost;Initial Catalog=OMMOF;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

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
            var MathOperator = "";
            var Instance = await _context.CTaps_UploadInstance.Where(r => r.UploadInstanceID == InstanceId).FirstOrDefaultAsync();
            if (Instance != null)
            {
                ViewData["InstanceId"] = Instance.UploadInstanceID;
                var insData = new
                {
                    MonthName = await _context.BS_Month.Where(r => r.MonthID == Instance.MonthId).Select(r => r.MonthName.ToString().Trim()).FirstOrDefaultAsync(),
                    Year = Instance.Year,
                    Site = Instance.Site.Trim(),
                    DateCreated = Instance.DateCreated
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

            if(status is not null)
            {
                var readingsList = await GetReadingsByType(status,InstanceId);
                if(status == "all")
                {
                    ViewBag.AllReadingsCount = readingsList.Count();
                }
                if (status == "negative")
                {
                    ViewBag.NReadingsCount = readingsList.Count();
                }
                if (status == "zero")
                {
                    ViewBag.ZReadingsCount = readingsList.Count();
                }
                if (status == "average")
                {
                    ViewBag.AReadingsCount = readingsList.Count();
                }

                return View((IEnumerable<CTaps_Reading>)readingsList);
            }
            
            var items = await _context.CTaps_Reading.Where(m => m.UploadInstanceId == InstanceId).ToListAsync();
            return View(items);

        }

        public async Task<List<CTaps_Reading>> GetReadingsByType(string ReadingType,int InstanceId)
        {
            string Operand = "";
            string Option ="";

            if (ReadingType == "all")
            {
                Operand = "> WRED.PREVIOUS_READING OR WRED.CURRENT_READING < WRED.PREVIOUS_READING OR WRED.CURRENT_READING = WRED.PREVIOUS_READING OR WRED.PREVIOUS_READING =";
                Option = "OR  WRED.CURRENT_READING >=  WRED.PREVIOUS_READING OR WRED.CURRENT_READING =  WRED.PREVIOUS_READING  ";

            }
            if (ReadingType == "negative")
            {
                Operand = "<";
            }
            if (ReadingType == "zero")
            {
                Operand = "=";
            }
            if (ReadingType == "average")
            {
                Operand = ">=";
            }

            //Find the billing instance using the city tap instance record
            var ctapsInstance = await _context.CTaps_UploadInstance.Where(r => r.UploadInstanceID == InstanceId).FirstOrDefaultAsync();
            var billingInstance = await _context.BS_WaterReadingExport.Where(r => r.MonthID == ctapsInstance.MonthId && r.Year == ctapsInstance.Year).Select(r => r.WaterReadingExportID).FirstOrDefaultAsync();


            #region Query
            string query = $"SELECT DISTINCT \n" +
                           $"    WRED.CUSTOMER_NUMBER,\n" +
                           $"    CT.CUSTNMBR_CITYTAP as CITY_TAP_CUSTOMER_NUMBER,\n" +
                           $"    [Timestamp],\n" +
                           $"    WRED.CUSTOMER_NAME,\n" +
                           $"    WRED.ERF_NUMBER,\n" +
                           $"    WRED.METER_NUMBER,\n" +
                           $"    WRED.AREA,\n" +
                           $"    WRED.PREVIOUS_READING,\n" +
                           $"    WRED.CURRENT_READING\n  \nFROM \n" +
                           $"    BS_WaterReadingExportData WRED\n" +
                           $"   JOIN CTaps_Reading CTR ON WRED.CUSTOMER_NUMBER = (SELECT  CUSTNMBR FROM BS_DebtorCityTap WHERE CUSTNMBR_CITYTAP = CTR.CustomerNo)\n" +
                           $"   JOIN \n    BS_DebtorCityTap CT \n    ON WRED.CUSTOMER_NUMBER = CT.CUSTNMBR \nWHERE  \n" +
                           $"    WRED.IsCityTab = 1 \n" +
                           $"    AND WRED.WaterReadingExportID = {billingInstance}\n" +
                           $"    AND WRED.CURRENT_READING {Operand} WRED.PREVIOUS_READING;";
            #endregion

            List<CTaps_Reading> cTaps_ReadingList = new List<CTaps_Reading>();
            try
            {
                using (SqlConnection con = new SqlConnection(SqlConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        CTaps_Reading cTaps_Reading = new CTaps_Reading();

                        cTaps_Reading.CustomerNo = rdr["CITY_TAP_CUSTOMER_NUMBER"].ToString().Trim();
                        cTaps_Reading.Timestamp = (DateTime)rdr["TIMESTAMP"];
                        cTaps_Reading.PreviousReading = (int?)(decimal?)rdr["PREVIOUS_READING"];
                        cTaps_Reading.Reading = (int?)(decimal?)rdr["CURRENT_READING"];
                        cTaps_Reading.MeterSerial = rdr["METER_NUMBER"].ToString();

                        // Check if an object with the same unique properties already exists
                        bool exists = cTaps_ReadingList.Any(existing => existing.CustomerNo == cTaps_Reading.CustomerNo.Trim());

                        if (exists)
                        {
                            continue;
                        }


                        cTaps_ReadingList.Add(cTaps_Reading);
                    }

                    return cTaps_ReadingList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
