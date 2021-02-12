using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogFilterWeb.Models.Cookie;
using LogFilterWeb.Models.Domain;
using LogFilterWeb.Utility;

namespace LogFilterWeb.Services
{
    public static class SmartUCFService
    {
        public static IEnumerable<StopwatchRecord> GetStopwatchRecordsForRange(SmartUCF cookieData, ref dynamic meta)
        {
            meta.begin = DateTime.Now.ToLocalTime();

            var activeServers = cookieData.MonitoredServers;
            var from = cookieData.StartDate.Date;
            var to = cookieData.EndDate.Date;

            bool fromCache;
            var smartUCFRoute = FilesHelper.GetSmartUCFRoute(Constants.SmartUCFDefaultConfig);
            var csvFiles = FilesHelper.GetFilesFromDirectory(new DirectoryInfo(smartUCFRoute), "[stopwatch]-lists.csv", out fromCache);

            var filesInRange = csvFiles.Where(x => // config/machineName/yyyy-MM-dd/[stopwatch]-lists.csv
                x.Directory?.Parent != null && 
                activeServers.Contains(x.Directory.Parent.Name) &&
                FilesHelper.ToDateTime(x.Directory.Name) >= from.Date &&
                FilesHelper.ToDateTime(x.Directory.Name) <= to.Date
            );

            // NOTE: Do try to avoid this
            var filesInRangeArray = filesInRange as FileInfo[] ?? filesInRange.ToArray();
            var stopwatchRecords = filesInRangeArray.Select(FilesHelper.ReadStopWatchRecords)
                .SelectMany(x => x);

            /*var stopwatchRecords = new List<StopwatchRecord>();
            foreach (var fileInfo in filesInRange)
            {
                stopwatchRecords.AddRange(FilesHelper.ReadStopWatchRecords(fileInfo.FullName));
            }*/

            meta.from = from;
            meta.fromCache = fromCache;
            meta.config = Constants.SmartUCFDefaultConfig;
            meta.files = filesInRangeArray.Select(x => x.FullName);
            meta.to = to;

            meta.end = DateTime.Now.ToLocalTime();
            meta.elapsed = (meta.end - meta.begin).ToString("c");

            return stopwatchRecords;
        }
    }
}
