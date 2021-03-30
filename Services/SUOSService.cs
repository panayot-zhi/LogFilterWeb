using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogFilterWeb.Models.Cookie;
using LogFilterWeb.Models.Domain;
using LogFilterWeb.Utility;
using NUglify.Helpers;

namespace LogFilterWeb.Services
{
    public class SUOSService
    {
        public static IEnumerable<SummaryFile> GetSummaryRecords(SUOS cookieData, string config, ref dynamic meta, string filter = null)
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

            var forSpecificFilter = !string.IsNullOrEmpty(filter);
            var filesInRangeArray = filesInRange as FileInfo[] ?? filesInRange.ToArray();
            var summaryRecords = filesInRangeArray.Select(FilesHelper.ReadSummary);
            if (forSpecificFilter)
            {
                summaryRecords = summaryRecords.Select(x => new SummaryFile()
                {
                    Date = x.Date,
                    MachineName = x.MachineName,
                    InputFile = x.InputFile,
                    BeginDateTime = x.BeginDateTime,
                    BeginProcessTimestamp = x.BeginProcessTimestamp,
                    CopyOriginal = x.CopyOriginal,
                    Elapsed = x.Elapsed,
                    EndDateTime = x.EndDateTime,
                    EndProcessTimestamp = x.EndProcessTimestamp,
                    EntriesConstructed = x.EntriesConstructed,
                    FilesRead = x.FilesRead,
                    FilesWritten = x.FilesWritten,
                    FilteredEntries = x.FilteredEntries,
                    InputFolder = x.InputFolder,
                    LinesRead = x.LinesRead,
                    LinesWritten = x.LinesWritten,
                    LogsRead = x.LogsRead,
                    NonStandardEntries = x.NonStandardEntries,
                    OutputFolder = x.OutputFolder,
                    OverwriteFiles = x.OverwriteFiles,
                    ParserName = x.ParserName,
                    Reparse = x.Reparse,
                    SplitByIdentities = x.SplitByIdentities,
                    SplitByLogLevels = x.SplitByLogLevels,
                    SplitByThreads = x.SplitByThreads,
                    TakeLastFiles = x.TakeLastFiles,
                    VerboseMode = x.VerboseMode,
                    
                    Filters = x.Filters.Where(f => f.Name == filter).ToArray()

                });
            }

            var summaryRecordsArray = summaryRecords as SummaryFile[] ?? summaryRecords.ToArray();
            //var summaryAggregate = FilesHelper.Combine(summaryRecords);
            //summaryRecords.Add(summaryAggregate);

            meta.from = from;
            meta.fromCache = fromCache;
            meta.config = config;
            meta.records = summaryRecordsArray.FirstOrDefault()?.Filters.Length;
            meta.logFiles = summaryRecordsArray.Select(x => x.InputFile);
            meta.summaryFiles = filesInRangeArray.Select(x => x.FullName);
            meta.to = to;

            meta.end = DateTime.Now.ToLocalTime();
            meta.elapsed = (meta.end - meta.begin).ToString("c");

            return summaryRecordsArray;
        }

        public static IEnumerable<UserQueryFile> GetUserQueryData(SUOS cookieData, string serviceName, ref dynamic meta, string user = null)
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

            var forSpecificUser = !string.IsNullOrEmpty(user);
            var userQueryFilesQuery = filesInRange.Select(FilesHelper.ReadUserQueryFiles);
            if (forSpecificUser)
            {
                userQueryFilesQuery = userQueryFilesQuery.Select(x => new UserQueryFile()
                {
                    Date = x.Date,
                    FullName = x.FullName,
                    MachineName = x.MachineName,
                    Records = x.Records
                        .Where(r => r.User == user)
                        .Cast<UserQueryRecordExtended>()
                });
            }

            var userQueryFiles = userQueryFilesQuery.ToList();

            meta.from = from;
            meta.fromCache = fromCache;
            meta.records = userQueryFiles.SelectMany(x => x.Records).DistinctBy(x => x.User).Count();
            meta.config = Constants.SmartUCFDefaultConfig;
            meta.files = userQueryFiles.Select(x => x.FullName);
            meta.to = to;

            meta.end = DateTime.Now.ToLocalTime();
            meta.elapsed = (meta.end - meta.begin).ToString("c");

            return userQueryFiles;
        }
    }
}
