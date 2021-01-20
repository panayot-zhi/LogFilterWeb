using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogFilterWeb.Models.View;
using LogFilterWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogFilterWeb.Controllers
{
    public class SmartUCFController : Controller
    {
        public IActionResult Index()
        {
            var vModel = new SmartUCF()
            {
                StopwatchFiles = SmartUCFService.GetOverallStopwatchData(DateTime.Now.AddDays(-7), DateTime.Now)
            };

            return View(vModel);
        }
    }
}
