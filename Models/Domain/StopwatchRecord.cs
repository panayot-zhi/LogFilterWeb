using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogFilterWeb.Models.Domain
{
    public class StopwatchRecord
    {
        public DateTime Date { get; set; }

        public string User { get; set; }

        public string ListName { get; set; }

        public int NumberOfRows { get; set; }

        public int RetrieveMilliseconds { get; set; }
    }
}
