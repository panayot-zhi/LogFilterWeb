using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogFilterWeb.Controllers.Api
{
    [ApiController]
    public class ValuesApiController : ControllerBase
    {
        [HttpGet]
        [Route("api/values/ping")]
        public string Ping(string ping)
        {
            return $"{DateTime.Now}: " + ping;
        }
    }
}
