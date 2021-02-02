using LogFilterWeb.Utility;

namespace LogFilterWeb.Models.Cookie
{
    public class SmartUCF : DateRange
    {
        public static readonly string CookieName = "SmartUCFConfig";

        public string[] MonitoredServers { get; set; } = Constants.SmartUCFMachines;
    }
}
