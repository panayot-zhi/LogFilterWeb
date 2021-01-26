using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogFilterWeb.Utility
{
    public class DateRange
    {
        public DateTime StartDate { get; set; } = DateTime.Now.Date.AddDays(-7);

        public DateTime EndDate { get; set; } = DateTime.Now.Date;
    }
}
