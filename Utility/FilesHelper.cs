using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace LogFilterWeb.Utility
{
    public static class FilesHelper
    {
        public static bool ThrowIfFileNameNotDate = true;
        public static bool UseFileContentsCaching = true;

        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions()
        {
            ExpirationScanFrequency = TimeSpan.FromHours(1)
        });

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

        public static T ReadJson<T>(string filePath)
        {
            var key = Hash(filePath);

            // check cache for entry
            var cacheEntry = Cache.Get<T>(key);
            if (cacheEntry != null && UseFileContentsCaching)
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

        public static string GetMinProcessTimestamp(string a, string b)
        {
            return Nullable.Compare(ToDateTime(a), ToDateTime(b)) < 0 ? a : b;
        }

        public static DateTime? ToDateTime(string timestamp)
        {
            if (DateTime.TryParseExact(timestamp,
                format: Constants.TimestampFormat,
                provider: CultureInfo.InvariantCulture,
                style: DateTimeStyles.None,
                out var result))
            {
                return result;
            }

            return null;

        }

        public static DateTime? GetFileNameAsDate(string fileName)
        {
            const string fileDateFormat = Constants.FileDateFormat;
            if (DateTime.TryParseExact(fileName, fileDateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var result))
            {
                return result;
            }

            if (ThrowIfFileNameNotDate)
            {
                throw new ArgumentException($"The specified argument '{fileName}' is not a date in the format '{fileDateFormat}'.");
            }

            return null;
        }

        public static long GetFileSize(string filePath)
        {
            return new FileInfo(filePath).Length;
        }

        public static string GetFileName(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
        }

        public static string GetFileNameAndExtension(string filePath)
        {
            return new FileInfo(filePath).Name;
        }

        public static string GetParentDirectoryName(string filePath)
        {
            return new FileInfo(filePath).Directory?.Name;
        }

        public static string GetDirectoryName(string filePath)
        {
            return new DirectoryInfo(filePath).Name;
        }
    }
}
