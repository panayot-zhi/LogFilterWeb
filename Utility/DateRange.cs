using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogFilterWeb.Utility
{
    public class DateRange
    {
        private DateTimeOffset _startDate = DateTime.Now.Date.AddDays(-7);
        private DateTimeOffset _endDate = DateTime.Now.Date;

        public DateTimeOffset StartDate
        {
            get => _startDate.ToLocalTime();
            set => _startDate = value;
        }

        public DateTimeOffset EndDate
        {
            get => _endDate.ToLocalTime();
            set => _endDate = value;
        }

        public bool SingleDate => StartDate.Date.Equals(EndDate.Date);
    }
}
