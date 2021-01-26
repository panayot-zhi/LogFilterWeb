using System;
using System.Collections.Generic;
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
    }
}
