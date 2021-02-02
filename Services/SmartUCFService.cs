using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogFilterWeb.Models.Domain;
using LogFilterWeb.Utility;

namespace LogFilterWeb.Services
{
    public static class SmartUCFService
    {
        public static IEnumerable<StopwatchRecord> GetStopwatchRecordsForRange(DateTime from, DateTime to, ref dynamic meta)
        {
            var smartUCFRoute = FilesHelper.GetSmartUCFRoute(Constants.SmartUCFDefaultConfig);
            var csvFiles = FilesHelper.GetFilesFromDirectory(new DirectoryInfo(smartUCFRoute), "[stopwatch]-lists.csv");

            var filesInRange = csvFiles.Where(x =>
                FilesHelper.ToDateTime(x.Directory.Name) >= from.Date &&
                FilesHelper.ToDateTime(x.Directory.Name) <= to.Date
            );

            var filesInRangeArray = filesInRange as FileInfo[] ?? filesInRange.ToArray();
            var stopwatchRecords = filesInRangeArray.Select(FilesHelper.ReadStopWatchRecords)
                .SelectMany(x => x);

            /*var stopwatchRecords = new List<StopwatchRecord>();
            foreach (var fileInfo in filesInRange)
            {
                stopwatchRecords.AddRange(FilesHelper.ReadStopWatchRecords(fileInfo.FullName));
            }*/

            meta.from = from;
            meta.config = Constants.SmartUCFDefaultConfig;
            meta.files = filesInRangeArray.Select(x => x.FullName);
            meta.machines = filesInRangeArray.Select(x => FilesHelper.GetSmartUCFMachineName(x.FullName, out _));
            meta.to = to;

            return stopwatchRecords;
        }

        /*
        public static IEnumerable<StopwatchFile> GetStopwatchFilesForRange(DateTime from, DateTime to)
        {
            var smartUCFRoute = Constants.GetSmartUCFRoute(Constants.SmartUCFDefaultConfig);
            var csvFiles = FilesHelper.GetFilesFromDirectory(new DirectoryInfo(smartUCFRoute), "[stopwatch]-lists.csv");

            var filesInRange = csvFiles.Where(x =>
                FilesHelper.ToDateTime(x.Directory.Name) >= from.Date &&
                FilesHelper.ToDateTime(x.Directory.Name) <= to.Date
            );

            var stopwatchFiles = filesInRange.Select(FilesHelper.ReadStopWatchFile);

            return stopwatchFiles;
        }
    */
    }
}
