using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogFilterWeb.Models.Domain
{
    public class UserQueryRecord
    {
        public int Count { get; set; }

        public IEnumerable<QueryRecord> Queries { get; set; }

        public string User { get; set; }
    }
}
