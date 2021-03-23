using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogFilterWeb.Models.Cookie;
using LogFilterWeb.Models.Domain;
using LogFilterWeb.Utility;

namespace LogFilterWeb.Services
{
    public class SUOSService
    {
        public static IEnumerable<SummaryFile> GetSummaryRecords(SUOS cookieData, string config, ref dynamic meta)
        {
            meta.begin = DateTime.Now.ToLocalTime();

            var activeServers = cookieData.MonitoredServers;
            var from = cookieData.StartDate.Date;
            var to = cookieData.EndDate.Date;

            bool fromCache;
            var suosRoute = FilesHelper.GetSUOSRoute(config);
            var summaryFiles = FilesHelper.GetFilesFromDirectory(new DirectoryInfo(suosRoute), $"[summary]-*.json", out fromCache);

            var filesInRange = summaryFiles.Where(x => // config/machineName/yyyy-MM-dd/[summary]-yyyy-MM-dd.json
                x.Directory?.Parent != null &&
                activeServers.Contains(x.Directory.Parent.Name) &&
                FilesHelper.ToDateTime(x.Directory.Name) >= from.Date &&
                FilesHelper.ToDateTime(x.Directory.Name) <= to.Date
            );

            var summaryRecords = filesInRange.Select(FilesHelper.ReadSummary).ToList();
            //var summaryAggregate = FilesHelper.Combine(summaryRecords);
            //summaryRecords.Add(summaryAggregate);

            meta.from = from;
            meta.fromCache = fromCache;
            meta.config = config;
            meta.files = summaryRecords.Select(x => x.InputFile);
            meta.to = to;

            meta.end = DateTime.Now.ToLocalTime();
            meta.elapsed = (meta.end - meta.begin).ToString("c");

            return summaryRecords;
        }
    }
}
