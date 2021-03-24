using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogFilterWeb.Models.Domain
{
    public class QueryRecord
    {
        public string Url { get; set; }

        public int Count { get; set; }

        public List<QueryRecord> Queries { get; set; }

        public string InstallmentCount { get; set; }

        public string OnlineProductCode { get; set; }

        public string OrderNo { get; set; }


        // extra fields
        [JsonExtensionData]
        private IDictionary<string, JToken> _additionalData;

        [JsonIgnore]
        public IDictionary<string, string> UnknownProperties
        {
            get { 
                return _additionalData.ToDictionary(keySelector: x => x.Key, 
                    elementSelector: x => x.Value.ToString());
            }
        }
    }
}