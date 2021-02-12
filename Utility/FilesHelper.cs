using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
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

        /// <summary>
        /// Returns files satisfying the search pattern within a given directory.
        /// This method uses cache to store the result file information from the combination of directory and search pattern (hashed). 
        /// </summary>
        /// <param name="directoryInfo">The directory to look in. The method looks in all subsequent directories.</param>
        /// <param name="searchPattern">The search string to match against the names of files. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <param name="fromCache">Am output flag that indicates whether or not the result was retrieved from the cache.</param>
        /// <returns>FileInfo's enumerated, could be from cache.</returns>
        public static IEnumerable<FileInfo> GetFilesFromDirectory(DirectoryInfo directoryInfo, string searchPattern, out bool fromCache)
        {
            var key = Hash($"{directoryInfo.FullName}, {searchPattern}");

            // check cache for entry
            var cacheEntry = Cache.Get<IEnumerable<FileInfo>>(key);
            if (cacheEntry != null && UseCache)
            {
                fromCache = true;

                // if we're allowed to use the cache,
                // and there is an entry retrieved
                return cacheEntry;
            }

            var result = directoryInfo.GetFiles(searchPattern, SearchOption.AllDirectories)
                .OrderByDescending(x => x.FullName).ToList(); // always order by descending FullName

            // refresh the record data in cache
            Cache.Set(key, result, TimeSpan.FromHours(1));

            fromCache = false;

            return result;
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
                Records = ReadStopWatchRecords(fullFileName)
            };
        }

        public static IEnumerable<StopwatchRecord> ReadStopWatchRecords(FileInfo fileInfo)
        {
            return ReadStopWatchRecords(fileInfo.FullName);
        }

        /// <summary>
        /// Reads a stopwatch csv file into a record set.
        /// This method uses cache to store the result records and uses the hashed file name as key.
        /// </summary>
        /// <param name="filePath">The full file path to the stopwatch csv file. This will be hashed and used as a cache key.</param>
        /// <returns>Enumerated stopwatch records, could be from cache.</returns>
        public static IEnumerable<StopwatchRecord> ReadStopWatchRecords(string filePath)
        {
            var machineName = GetSmartUCFMachineName(filePath, out _);

            var key = Hash(filePath);

            // check cache for entry
            var cacheEntry = Cache.Get<IEnumerable<StopwatchRecord>>(key);
            if (cacheEntry != null && UseCache)
            {
                // if we're allowed to use the cache,
                // and there is an entry retrieved
                return cacheEntry;
            }

            // else retrieve the data again
            var result = File.ReadLines(filePath)
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

                }).ToList();

            // refresh the record data in cache
            Cache.Set(key, result, TimeSpan.FromHours(1));

            return result;
        }

        private static string Hash(string input)
        {
            using (var md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
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

        public static string GetSmartUCFLogFilePath(string fullFilePath)
        {
            var machineName = GetSmartUCFMachineName(fullFilePath, out _);
            var fileName = new FileInfo(fullFilePath).Directory.Name + ".log";

            return Path.Combine(Constants.SmartUCFLogsRoot, machineName, fileName);
        }

        public static byte[] ZipSmartUCFCsvFiles(string[] filePaths)
        {
            using var memory = new MemoryStream();
            using (var zip = new ZipArchive(memory, ZipArchiveMode.Create, false, Encoding.UTF8))
            {
                foreach (var filePath in filePaths)
                {
                    var fileInfo = new FileInfo(filePath);
                    var directoryName = fileInfo.Directory.Name;
                    var machineName = GetSmartUCFMachineName(filePath, out _);
                    zip.Add(filePath, $"{directoryName}/file_{machineName}.csv");
                }
            }

            return memory.ToArray();
        }

        public static byte[] ZipSmartUCFLogFiles(string[] filePaths)
        {
            using var memory = new MemoryStream();
            using (var zip = new ZipArchive(memory, ZipArchiveMode.Create, false, Encoding.UTF8))
            {
                foreach (var filePath in filePaths)
                {
                    var logFilePath = GetSmartUCFLogFilePath(filePath);

                    var fileInfo = new FileInfo(logFilePath);
                    var machineName = fileInfo.Directory.Name;
                    zip.Add(logFilePath, $"{machineName}/{fileInfo.Name}");
                }
            }

            return memory.ToArray();
        }
    }
}
