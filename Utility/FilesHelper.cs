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

        public static SummaryFile ReadSummary(FileInfo fileInfo)
        {
            return ReadSummary(fileInfo.FullName);
        }

        public static SummaryFile ReadSummary(string filePath)
        {
            var machineName = GetSUOSMachineName(filePath, out _);

            var summary = ReadJson<SummaryFile>(filePath);

            summary.MachineName = machineName;

            return summary;
        }

        public static SummaryFile Combine(IEnumerable<SummaryFile> list)
        {
            SummaryFile seed = null;
            foreach (var summary in list)
            {
                if (seed == null)
                {
                    seed = ObjectCloner.JsonCopy(summary);
                    seed.MachineName = null;
                    continue;
                }

                seed = Combine(seed, summary);
            }

            return seed;
        }

        public static SummaryFile Combine(SummaryFile source, SummaryFile other)
        {
            var result = ObjectCloner.JsonCopy(source);

            result.LinesRead += other.LinesRead;
            result.LogsRead += other.LogsRead;
            result.NonStandardEntries += other.NonStandardEntries;
            result.EntriesConstructed += other.EntriesConstructed;
            result.FilteredEntries += other.FilteredEntries;
            result.LinesWritten += other.LinesWritten;
            result.FilesWritten += other.FilesWritten;
            result.FilesRead += other.FilesRead;

            result.BeginProcessTimestamp = GetMinProcessTimestamp(source.BeginProcessTimestamp, other.BeginProcessTimestamp);
            result.EndProcessTimestamp = GetMinProcessTimestamp(source.EndProcessTimestamp, other.EndProcessTimestamp);

            result.Elapsed = TimeSpan.Parse(source.Elapsed).Add(TimeSpan.Parse(other.Elapsed)).ToString();

            result.InputFile = "-";

            foreach (var f in result.Filters)
            {
                f.Count += other.Filters.Single(x => x.Name == f.Name).Count;
            }

            return result;
        }

        public static UserQueryFile Combine(IEnumerable<UserQueryFile> list)
        {
            UserQueryFile seed = null;
            foreach (var queryFile in list)
            {
                if (seed == null)
                {
                    seed = ObjectCloner.JsonCopy(queryFile);
                    continue;
                }

                seed = Combine(seed, queryFile);
            }

            return seed;
        }

        public static UserQueryFile Combine(UserQueryFile source, UserQueryFile other)
        {
            var resultRecords = ObjectCloner.JsonCopy(source.Records).ToList();
            var differences = other.Records.Where(otherRecord =>
                resultRecords.All(sourceRecord => sourceRecord.User != otherRecord.User));

            // resultRecords.AddRange(other.Records.Except(source.Records));
            resultRecords.AddRange(differences);

            foreach (var resultRecord in resultRecords)
            {
                resultRecord.Count += other.Records.SingleOrDefault(x => x.User == resultRecord.User)?.Count ?? 0;
            }

            return new UserQueryFile()
            {
                Records = resultRecords
            };
        }

        public static string GetMinProcessTimestamp(string a, string b)
        {
            return ToDateTime(a) < ToDateTime(b) ? a : b;
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

        public static UserQueryFile ReadUserQueryFiles(FileInfo fileInfo, bool extended = false)
        {
            return ReadUserQueryFiles(fileInfo.FullName, extended);
        }

        public static UserQueryFile ReadUserQueryFiles(string filePath, bool extended = false)
        {
            var machineName = GetSUOSMachineName(filePath, out _);

            return new UserQueryFile()
            {
                Date = DateTime.Now,
                FullName = filePath,
                MachineName = machineName,
                Records = ReadUserQueryRecords(filePath)
            };
        }

        public static IEnumerable<UserQueryRecordBase> ReadUserQueryRecords(string filePath)
        {
            return ReadJson<IEnumerable<UserQueryRecordExtended>>(filePath).OrderByDescending(x => x.Count);
        }

        private static T ReadJson<T>(string filePath)
        {
            var key = Hash(filePath);

            // check cache for entry
            var cacheEntry = Cache.Get<T>(key);
            if (cacheEntry != null && UseCache)
            {
                // if we're allowed to use the cache,
                // and there is an entry retrieved
                return cacheEntry;
            }

            // else retrieve the data again
            var result = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));

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

        public static byte[] ZipSUOSSummaryFiles(string[] filePaths)
        {
            using var memory = new MemoryStream();
            using (var zip = new ZipArchive(memory, ZipArchiveMode.Create, false, Encoding.UTF8))
            {
                foreach (var filePath in filePaths)
                {
                    var fileInfo = new FileInfo(filePath);
                    var directoryName = fileInfo.Directory.Name;
                    var machineName = fileInfo.Directory.Parent.Name;

                    zip.Add(filePath, $"{directoryName}/summary_{machineName}.json");
                }
            }

            return memory.ToArray();
        }

        public static byte[] ZipSUOSServiceJsonFiles(string[] filePaths, string fullServiceName)
        {
            using var memory = new MemoryStream();
            using (var zip = new ZipArchive(memory, ZipArchiveMode.Create, false, Encoding.UTF8))
            {
                foreach (var filePath in filePaths)
                {
                    var fileInfo = new FileInfo(filePath);
                    var directoryName = fileInfo.Directory.Name;
                    var machineName = fileInfo.Directory.Parent.Name;

                    var targetFilePath = filePath.Replace(fileInfo.Name, $"{fullServiceName}.json");
                    zip.Add(targetFilePath, $"{directoryName}/{fullServiceName}_{machineName}.json");
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
                    var machineName = GetSmartUCFMachineName(filePath, out _);
                    var fileName = new FileInfo(filePath).Directory.Name + ".log";
                    var logFilePath = Path.Combine(Constants.SmartUCFLogsRoot, machineName, fileName);
                    var fileInfo = new FileInfo(logFilePath);

                    zip.Add(logFilePath, $"{machineName}/{fileInfo.Name}");
                }
            }

            return memory.ToArray();
        }

        public static byte[] ZipSUOSFilteredLogFiles(string[] filePaths)
        {
            using var memory = new MemoryStream();
            using (var zip = new ZipArchive(memory, ZipArchiveMode.Create, false, Encoding.UTF8))
            {
                foreach (var filePath in filePaths)
                {
                    var fileInfo = new FileInfo(filePath);
                    var directoryName = fileInfo.Directory.Name;
                    var machineName = fileInfo.Directory.Parent.Name;

                    var targetFilePath = filePath.Replace(fileInfo.Name, $"[filtered]-{directoryName}.log");
                    zip.Add(targetFilePath, $"{directoryName}/filtered_{machineName}.log");
                }
            }

            return memory.ToArray();
        }

        public static byte[] ZipSUOSLogFiles(string[] filePaths)
        {
            using var memory = new MemoryStream();
            using (var zip = new ZipArchive(memory, ZipArchiveMode.Create, false, Encoding.UTF8))
            {
                foreach (var filePath in filePaths)
                {
                    var machineName = GetSUOSMachineName(filePath, out _);
                    var fileInfo = new FileInfo(filePath);

                    zip.Add(filePath, $"{machineName}/{fileInfo.Name}");
                }
            }

            return memory.ToArray();
        }
    }
}
