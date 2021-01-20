using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogFilterWeb.Models.Domain;
using LogFilterWeb.Utility;

namespace LogFilterWeb.Services
{
    public static class SmartUCFService
    {
        public static IEnumerable<StopwatchFile> GetOverallStopwatchData(DateTime from, DateTime to)
        {
            // TODO: Remove ToList here
            var smartUCFRoute = Constants.GetSmartUCFRoute(Constants.SmartUcfDefaultConfig);
            var csvFiles = FilesHelper.GetFilesFromDirectory(new DirectoryInfo(smartUCFRoute), "[stopwatch]-lists.csv").ToList();
            
            var filesInRange = csvFiles.Where(x =>
                FilesHelper.ToDateTime(x.Directory.Name) >= from.Date &&
                FilesHelper.ToDateTime(x.Directory.Name) <= to.Date
            ).ToList();

            var stopwatchFiles = filesInRange.Select(FilesHelper.ReadStopWatchFile).ToList();

            return stopwatchFiles;
        }
    }
}
