using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace LogFilterWeb.Models.Domain
{
    [Serializable]
    public class SummaryFilter
    {
        /// <summary>
        /// Specifies the human-friendly name of this filter.
        /// It may be included in the output file name if WriteToFile flag is true.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Short explanatory description about what this filter does.
        /// It can be used only for counting or for file separation etc.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Specifies whether or not the filtered line should be
        /// included or excluded in the result set after the filter is applied.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Incremented whenever this occurrence's
        /// filter is matched upon a log entry.
        /// </summary>
        public ulong Count { get; set; } = 0;

        /// <summary>
        /// Specifies number of log entries surrounding the matched entry by
        /// this filter that should be included in the result set.
        /// </summary>
        public int Context { get; set; } = 0; // no context by default

        /// <summary>
        /// For which property of the log entry should this filter be applied on.
        /// If this property is null the value of the filter will be applied on the original line.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// The filter value to be matched against the value of the log entry property.
        /// </summary>
        public Filter Value { get; set; }
    }

    public class Filter
    {
        public string Pattern { get; set; }
    }
}