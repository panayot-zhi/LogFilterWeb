using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogFilterWeb.Utility
{
    public static class Constants
    {
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss,fff
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss,fff";


        /// <summary>
        /// X:\\Processed\\SUOS\\
        /// </summary>
        public const string SUOSRoot = "X:\\Processed\\SUOS\\";

        /// <summary>
        /// X:\\Logs\\SUOS\\
        /// </summary>
        public const string SUOSLogsRoot = "X:\\Logs\\SUOS\\";

        /// <summary>
        /// "6", "7"
        /// </summary>
        public static readonly string[] SUOSMachines = { "6", "7" };

        /// <summary>
        /// otp
        /// </summary>
        public const string SUOSOtpConfig = "otp";

        /// <summary>
        /// buster
        /// </summary>
        public const string SUOSBusterConfig = "buster";

        /// <summary>
        /// default
        /// </summary>
        public const string SUOSDefaultConfig = "default";

        public static readonly string[] SUOSConfigurations = { SUOSDefaultConfig, SUOSBusterConfig, SUOSOtpConfig };

        public static string GetSUOSRoute(string config = null, string machine = null, string date = null)
        {
            var paths = new List<string>()
            {
                SUOSRoot
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

        public static string GetSUOSMachineName(string fullFilePath, out string config)
        {
            var supportedMachines = string.Join("|", SUOSMachines);
            var targetFilePath = fullFilePath.Replace(SUOSRoot, string.Empty);

            var machineFinderRegex = Regex.Match(targetFilePath, $"(?<Config>\\w+)\\\\(?<Machine>({supportedMachines}))\\\\");

            config = machineFinderRegex.Groups["Config"].Value;

            return machineFinderRegex.Groups["Machine"].Value;
        }





        /// <summary>
        /// X:\\Processed\\SmartUCF\\
        /// </summary>
        public const string SmartUCFRoot = "X:\\Processed\\SmartUCF\\";

        /// <summary>
        /// X:\\Logs\\SmartUCF\\
        /// </summary>
        public const string SmartUCFLogsRoot = "X:\\Logs\\SmartUCF\\";

        /// <summary>
        /// "06", "07", "08", "09"
        /// </summary>
        public static readonly string[] SmartUCFMachines =
        {
            "01", "02", "04", "05", // TODO: Remove this after testing!
            "06", "07", "08", "09"
        };

        /// <summary>
        /// default
        /// </summary>
        public const string SmartUCFDefaultConfig = "default";

        public static readonly string[] SmartUCFConfigurations = { SmartUCFDefaultConfig };

        public static string GetSmartUCFRoute(string config = null, string machine = null, string date = null)
        {
            var paths = new List<string>()
            {
                SmartUCFRoot
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
            var supportedMachines = string.Join("|", SmartUCFMachines);
            var targetFilePath = fullFilePath.Replace(SmartUCFRoot, string.Empty);

            var machineFinderRegex = Regex.Match(targetFilePath, $"(?<Config>\\w+)\\\\(?<Machine>({supportedMachines}))\\\\");

            config = machineFinderRegex.Groups["Config"].Value;

            return machineFinderRegex.Groups["Machine"].Value;
        }
    }
}
