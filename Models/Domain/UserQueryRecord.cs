using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogFilterWeb.Models.Domain
{
    public class UserQueryRecordBase
    {
        public string User { get; set; }

        public int Count { get; set; }
    }

    public class UserQueryRecordExtended : UserQueryRecordBase
    {
        public IEnumerable<QueryRecord> Queries { get; set; }
    }
}
