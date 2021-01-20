using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LogFilterWeb.Models.Domain
{
    public class StopwatchFile
    {
        public DateTime Date { get; set; }

        public string FullName { get; set; }

        public string ServerName { get; set; }

        public IEnumerable<StopwatchRecord> Records { get; set; }
    }
}
