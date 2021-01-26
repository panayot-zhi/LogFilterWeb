using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LogFilterWeb.Utility
{
    public static class Extensions
    {
        public static T ReadCookie<T>(this ControllerBase controller, string key) where T : new()
        {
            var cookieData = controller.HttpContext.Request.Cookies[key];

            if (cookieData == null)
            {
                return new T();
            }

            return JsonConvert.DeserializeObject<T>(cookieData);
        }
    }
}
