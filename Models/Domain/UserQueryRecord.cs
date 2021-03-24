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

        public bool Equals(UserQueryRecordBase other)
        {
            if (other is null)
                return false;

            return this.User == other.User;
        }

        public override bool Equals(object obj) => Equals(obj as UserQueryRecordBase);

        public override int GetHashCode() => (this.User).GetHashCode();
    }

    public class UserQueryRecordExtended : UserQueryRecordBase
    {
        public IEnumerable<QueryRecord> Queries { get; set; }
    }
}
