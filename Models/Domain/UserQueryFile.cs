using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogFilterWeb.Models.Domain
{
    public class UserQueryFile
    {
        public DateTime Date { get; set; }

        public string FullName { get; set; }

        public string MachineName { get; set; }

        public IEnumerable<UserQueryRecordBase> Records { get; set; }
    }
}
