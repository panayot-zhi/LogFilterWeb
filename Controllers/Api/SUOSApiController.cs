using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LogFilterWeb.Models.Cookie;
using LogFilterWeb.Models.Domain;
using LogFilterWeb.Services;
using LogFilterWeb.Utility;
using NUglify.JavaScript.Syntax;

namespace LogFilterWeb.Controllers.Api
{
    [ApiController]
    [Route("api/suos/[action]")]
    public class SUOSApiController : ControllerBase
    {
        [HttpGet]
        public dynamic GetSummariesData(string config = Constants.SUOSDefaultConfig)
        {
            dynamic meta = new ExpandoObject();

            var cookieData = this.ReadCookie<SUOS>(SUOS.CookieName);
            var data = SUOSService.GetSummaryRecords(cookieData, config, ref meta);
            //var query = data.AsParallel();

            return new
            {
                Meta = meta,
                Results = data.GroupBy(keySelector: x => x.MachineName, 
                    resultSelector: (machineName, groupByMachine) =>
                    {
                        return new
                        {
                            Machine = machineName,
                            Filters = FilesHelper.Combine(groupByMachine).Filters
                        };
                    } )
            };
        }
    }
}
