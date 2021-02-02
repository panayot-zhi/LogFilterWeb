using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LogFilterWeb.Models.Domain;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace LogFilterWeb.Utility
{
    public static class FilesHelper
    {
        private static bool ThrowIfNotDate = true;
        public static bool UseCache = true;

        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions()
        {
            ExpirationScanFrequency = TimeSpan.FromHours(1)
        });

        public static IEnumerable<FileInfo> GetFilesFromDirectory(DirectoryInfo directoryInfo, string searchPattern)
        {
            return directoryInfo.GetFiles(searchPattern, SearchOption.AllDirectories)
                .OrderByDescending(x => x.FullName); // always order by descending FullName
        }

        public static StopwatchFile ReadStopWatchFile(FileInfo fileInfo)
        {
            var fullFileName = fileInfo.FullName;
            var directoryName = fileInfo.Directory.Name;

            return new StopwatchFile()
            {
                FullName = fullFileName,
                Date = ToDateTime(directoryName),
                ServerName = GetSmartUCFMachineName(fullFileName,out _),
                Records = ReadStopWatchRecords(fullFileName).ToList()   // TODO: Remove ToList
            };
        }

        public static IEnumerable<StopwatchRecord> ReadStopWatchRecords(FileInfo fileInfo)
        {
            return ReadStopWatchRecords(fileInfo.FullName);
        }

        public static IEnumerable<StopwatchRecord> ReadStopWatchRecords(string filePath)
        {
            var machineName = GetSmartUCFMachineName(filePath, out _);

            return File.ReadLines(filePath)
                .Select(line => line.Split(',')
                    .Select(x => x.Trim()).ToArray())
                .Select(columns => new StopwatchRecord()
                {
                    Date = ToDateTime(columns[0]),
                    MachineName = machineName,
                    User = columns[1],
                    ListName = columns[2],
                    NumberOfRows = int.Parse(columns[3]),
                    RetrieveMilliseconds = int.Parse(columns[4])

                });
        }

        public static DateTime ToDateTime(string target)
        {
            var supportedFormats = new[]
            {
                Constants.DateFormat, 
                Constants.DateTimeFormat
            };

            if (DateTime.TryParseExact(target,
                formats: supportedFormats,
                provider: CultureInfo.InvariantCulture,
                style: DateTimeStyles.None,
                out var result))
            {
                return result;
            }

            if (ThrowIfNotDate)
            {
                throw new ArgumentException($"The specified argument '{target}' is not a date in the supported formats: '{string.Join(", ", supportedFormats)}'.");
            }

            return default;
        }

        public static string GetSmartUCFRoute(string config = null, string machine = null, string date = null)
        {
            var paths = new List<string>()
            {
                Constants.SmartUCFRoot
            };

            if (!string.IsNullOrEmpty(config))
            {
                paths.Add(config);
            }

            if (!string.IsNullOrEmpty(machine))
            {
                paths.Add(machine);
            }

            if (!string.IsNullOrEmpty(date))
            {
                paths.Add(date);
            }

            return Path.Combine(paths.ToArray());
        }

        public static string GetSmartUCFMachineName(string fullFilePath, out string config)
        {
            var supportedMachines = string.Join("|", Constants.SmartUCFMachines);
            var targetFilePath = fullFilePath.Replace(Constants.SmartUCFRoot, string.Empty);

            var machineFinderRegex = Regex.Match(targetFilePath, $"(?<Config>\\w+)\\\\(?<Machine>({supportedMachines}))\\\\");

            config = machineFinderRegex.Groups["Config"].Value;

            return machineFinderRegex.Groups["Machine"].Value;
        }

        public static string GetSUOSMachineName(string fullFilePath, out string config)
        {
            var supportedMachines = string.Join("|", Constants.SUOSMachines);
            var targetFilePath = fullFilePath.Replace(Constants.SUOSRoot, string.Empty);

            var machineFinderRegex = Regex.Match(targetFilePath, $"(?<Config>\\w+)\\\\(?<Machine>({supportedMachines}))\\\\");

            config = machineFinderRegex.Groups["Config"].Value;

            return machineFinderRegex.Groups["Machine"].Value;
        }

        public static string GetSUOSRoute(string config = null, string machine = null, string date = null)
        {
            var paths = new List<string>()
            {
                Constants.SUOSRoot
            };

            if (!string.IsNullOrEmpty(config))
            {
                paths.Add(config);
            }

            if (!string.IsNullOrEmpty(machine))
            {
                paths.Add(machine);
            }

            if (!string.IsNullOrEmpty(date))
            {
                paths.Add(date);
            }

            return Path.Combine(paths.ToArray());
        }
    }
}
