using System;
using System.Collections.Generic;
using System.Globalization;
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
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                // DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
            };

            if (cookieData == null)
            {
                return new T();
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(cookieData, jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while trying to parse {key} cookie: {ex.Message}", ex);
                controller.HttpContext.Response.Cookies.Delete(key);
                return new T();
            }
        }
    }
}
