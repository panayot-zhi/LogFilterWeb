using LogFilterWeb.Utility;

namespace LogFilterWeb.Models.Cookie
{
    public class SUOS : DateRange
    {
        public static readonly string CookieName = "SUOSConfig";

        public string[] MonitoredServers { get; set; } = Constants.SUOSMachines;
    }
}
