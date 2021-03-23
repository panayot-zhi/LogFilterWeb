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

        public IActionResult GetCsvZip(string[] filePaths)
        {
            throw new NotImplementedException();
        }

        public IActionResult GetLogsZip(string[] filePaths)
        {
            throw new NotImplementedException();
        }
    }
}
