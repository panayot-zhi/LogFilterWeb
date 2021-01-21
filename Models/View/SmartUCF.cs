using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogFilterWeb.Models.Domain;

namespace LogFilterWeb.Models.View
{
    public class SmartUCF
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string CurrentList { get; set; }

        public IEnumerable<StopwatchFile> StopwatchFiles { get; set; }
    }
}
