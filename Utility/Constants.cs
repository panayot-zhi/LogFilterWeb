using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogFilterWeb.Utility
{
    public static class Constants
    {
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public const string FileDateFormat = "yyyy-MM-dd";

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss,fff
        /// </summary>
        public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss,fff";


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



        /// <summary>
        /// X:\\Processed\\SmartUcf\\
        /// </summary>
        public const string SmartUcfRoot = "X:\\Processed\\SmartUcf\\";

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
    }
}
