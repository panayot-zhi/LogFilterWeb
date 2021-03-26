using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogFilterWeb.Models.Cookie;
using LogFilterWeb.Utility;
using Microsoft.AspNetCore.Mvc;

namespace LogFilterWeb.Controllers
{
    public class SUOSController : Controller
    {
        public IActionResult Index()
        {
            var vModel = this.ReadCookie<SUOS>(SUOS.CookieName);

            // TODO: Fill other viewModel properties

            return View(vModel);
        }

        public IActionResult GetSummariesZip(string[] filePaths)
        {
            var cookieData = this.ReadCookie<DateRange>(SUOS.CookieName);
            return new FileContentResult(FilesHelper.ZipSUOSSummaryFiles(filePaths), "application/octet-stream")
            {
                FileDownloadName = $"{cookieData.StartDate:yyyy-MM-dd}_SUMMARIES_{cookieData.EndDate:yyyy-MM-dd}.zip"
            };
        }

        public IActionResult GetServiceJsonZip(string[] filePaths, string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException(nameof(serviceName));
            }

            var cookieData = this.ReadCookie<DateRange>(SUOS.CookieName);
            var fullServiceName = serviceName?.ToLowerInvariant() + "requests";
            return new FileContentResult(FilesHelper.ZipSUOSServiceJsonFiles(filePaths, fullServiceName), "application/octet-stream")
            {
                FileDownloadName = $"{cookieData.StartDate:yyyy-MM-dd}_{serviceName.ToUpperInvariant()}_{cookieData.EndDate:yyyy-MM-dd}.zip"
            };
        }

        public IActionResult GetFilteredLogsZip(string[] filePaths)
        {
            var cookieData = this.ReadCookie<DateRange>(SUOS.CookieName);
            return new FileContentResult(FilesHelper.ZipSUOSFilteredLogFiles(filePaths), "application/octet-stream")
            {
                FileDownloadName = $"{cookieData.StartDate:yyyy-MM-dd}_FILTERED_{cookieData.EndDate:yyyy-MM-dd}.zip"
            };
        }

        public IActionResult GetLogsZip(string[] filePaths)
        {
            var cookieData = this.ReadCookie<DateRange>(SUOS.CookieName);
            return new FileContentResult(FilesHelper.ZipSUOSLogFiles(filePaths), "application/octet-stream")
            {
                FileDownloadName = $"{cookieData.StartDate:yyyy-MM-dd}_LOGS_{cookieData.EndDate:yyyy-MM-dd}.zip"
            };
        }
    }
}
