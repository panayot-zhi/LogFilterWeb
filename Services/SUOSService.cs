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
            meta.records = summaryRecords.FirstOrDefault()?.Filters.Length;
            meta.files = summaryRecords.Select(x => x.InputFile);
            meta.to = to;

            meta.end = DateTime.Now.ToLocalTime();
            meta.elapsed = (meta.end - meta.begin).ToString("c");

            return summaryRecords;
        }

        public static IEnumerable<UserQueryFile> GetUserQueryData(SUOS cookieData, string serviceName, ref dynamic meta)
        {
            meta.begin = DateTime.Now.ToLocalTime();

            var activeServers = cookieData.MonitoredServers;
            var from = cookieData.StartDate.Date;
            var to = cookieData.EndDate.Date;

            bool fromCache;
            var suosRoute = FilesHelper.GetSUOSRoute(Constants.SUOSDefaultConfig);
            var queryFiles = FilesHelper.GetFilesFromDirectory(new DirectoryInfo(suosRoute), $"{serviceName}.json", out fromCache);

            var filesInRange = queryFiles.Where(x => // default/machineName/yyyy-MM-dd/service.json
                x.Directory?.Parent != null &&
                activeServers.Contains(x.Directory.Parent.Name) &&
                FilesHelper.ToDateTime(x.Directory.Name) >= from.Date &&
                FilesHelper.ToDateTime(x.Directory.Name) <= to.Date
            );

            var userQueryFiles = filesInRange.Select(FilesHelper.ReadUserQueryFiles).ToList();

            meta.from = from;
            meta.fromCache = fromCache;
            meta.config = Constants.SmartUCFDefaultConfig;
            meta.files = userQueryFiles.Select(x => x.FullName);
            meta.to = to;

            meta.end = DateTime.Now.ToLocalTime();
            meta.elapsed = (meta.end - meta.begin).ToString("c");

            return userQueryFiles;
        }

    }
}
