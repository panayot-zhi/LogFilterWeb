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
        /// X:\\Processed\\SmartUcf\\
        /// </summary>
        public const string SmartUcfRoot = "X:\\Processed\\SmartUCF\\";

        /// <summary>
        /// X:\\Logs\\SmartUcf\\
        /// </summary>
        public const string SmartUcfLogsRoot = "X:\\Logs\\SmartUcf\\";

        /// <summary>
        /// "06", "07", "08", "09"
        /// </summary>
        public static readonly string[] SmartUcfMachines = { "06", "07", "08", "09" };

        /// <summary>
        /// default
        /// </summary>
        public const string SmartUcfDefaultConfig = "default";

        public static readonly string[] SmartUcfConfigurations = { SmartUcfDefaultConfig };

        public static string GetSmartUCFRoute(string config = null, string machine = null, string date = null)
        {
            var paths = new List<string>()
            {
                SmartUcfRoot
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
            var supportedMachines = string.Join("|", SmartUcfMachines);
            var targetFilePath = fullFilePath.Replace(SmartUcfRoot, string.Empty);

            var machineFinderRegex = Regex.Match(targetFilePath, $"(?<Config>\\w+)\\\\(?<Machine>({supportedMachines}))\\\\");

            config = machineFinderRegex.Groups["Config"].Value;

            return machineFinderRegex.Groups["Machine"].Value;
        }
    }
}
