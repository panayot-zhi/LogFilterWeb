using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogFilterWeb.Models.View;
using LogFilterWeb.Services;
using LogFilterWeb.Utility;
using Microsoft.AspNetCore.Mvc;

namespace LogFilterWeb.Controllers
{
    public class SmartUCFController : Controller
    {
        public IActionResult Index()
        {
            var vModel = this.ReadCookie<SmartUCF>(Constants.SmartUCFConfigCookieName);

            // TODO: Fill other viewModel properties

            return View(vModel);
        }

        public IActionResult GetCsvZip(string[] filePaths)
        {
            var cookieData = this.ReadCookie<DateRange>(Constants.SmartUCFConfigCookieName);
            return new FileContentResult(FilesHelper.ZipSmartUCFCsvFiles(filePaths), "application/octet-stream")
            {
                FileDownloadName = $"{cookieData.StartDate:yyyy-MM-dd}_CSV_{cookieData.EndDate:yyyy-MM-dd}.zip"
            };
        }

        public IActionResult GetLogsZip(string[] filePaths)
        {
            var cookieData = this.ReadCookie<DateRange>(Constants.SmartUCFConfigCookieName);
            return new FileContentResult(FilesHelper.ZipSmartUCFLogFiles(filePaths), "application/octet-stream")
            {
                FileDownloadName = $"{cookieData.StartDate:yyyy-MM-dd}_LOGS_{cookieData.EndDate:yyyy-MM-dd}.zip"
            };
        }
    }
}
