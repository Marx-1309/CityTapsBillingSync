 //[HttpGet]
 //public async Task<IActionResult> GetReadingsByUploadInstanceId(int InstanceId,string? status)
 //{
 //    var Instance = await _context.CTaps_UploadInstance.Where(r => r.UploadInstanceID == InstanceId).FirstOrDefaultAsync();
 //    if (Instance != null)
 //    {
 //        ViewData["InstanceId"] = Instance.UploadInstanceID;
 //        var insData = new
 //        {
 //            MonthName = await _context.BS_Month.Where(r => r.MonthID == Instance.MonthId).Select(r => r.MonthName.ToString().Trim()).FirstOrDefaultAsync(),
 //            Year = Instance.Year,
 //            Site = Instance.Site.Trim(),
 //            DateCreated = Instance.DateCreated
 //        };
 //        //var months = await _context.BS_Month.ToListAsync();

 //        var date = insData.DateCreated.ToString();

 //        char[] arrayStr = date.ToCharArray();

 //        var indx = Array.LastIndexOf(arrayStr, '/') + 5;
 //        TempData["instanceDetails"] =
 //                                        $" &nbsp;&nbsp;    Month : <strong> {insData.MonthName} </strong>" +
 //                                        $" &nbsp;&nbsp;    Year : <strong>{insData.Year}</strong>" +
 //                                        $" &nbsp;&nbsp;    Site : <strong>{insData.Site}</strong>" +
 //                                        $" &nbsp;&nbsp;    Date : <strong>{insData.DateCreated.ToString().Substring(0, indx)}</strong>";

 //        ViewBag.currentMonth = await _context.BS_Month.ToListAsync();

 //    }
 //    if(status == "all")
 //    {

 //        var previousInstId = _context.CTaps_Reading
 //            .Where(x => x.UploadInstanceId < InstanceId)
 //            .OrderByDescending(x => x.UploadInstanceId)
 //            .Select(x => x.UploadInstanceId)
 //            .FirstOrDefault();

 //        var CTapsCustNos = await _context.CTaps_Reading.Select(x => x.CustomerNo).Distinct().ToListAsync();

 //        var CTapsCustReadings = await _context.CTaps_Reading
 //                                .Where(m => m.UploadInstanceId == previousInstId && CTapsCustNos
 //                                .Contains(m.CustomerNo))
 //                                .ToListAsync();

 //        var allReadings = _context.BS_WaterReadingExportData.ToList();

 //        var zeroReadings = new List<CTaps_Reading>();

 //        //zeroReadings.AddRange(itemsAll.Where());

 //        foreach (var item in CTapsCustReadings)
 //        {
 //            var custNo = await _context.BS_DebtorCityTap.Where(r=>r.CUSTNMBR_CITYTAP.Equals(item.CustomerNo)).Select(r=>r.CUSTNMBR).FirstOrDefaultAsync();
 //            item.PreviousReading = _context.BS_WaterReadingExportData
 //            .Where(r => r.CUSTOMER_NUMBER == custNo && r.WaterReadingExportID == Instance.UploadInstanceID)
 //            .Select(r => r.PREVIOUS_READING)
 //            .Cast<int?>()
 //            .FirstOrDefault();

 //            zeroReadings.Add(item);
 //        }
         
 //        return View(zeroReadings);
 //    }
 //    var items = await _context.CTaps_Reading.Where(m => m.UploadInstanceId == InstanceId).ToListAsync();
 //    return View(items);

 //}