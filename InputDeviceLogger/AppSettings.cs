using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputDeviceLogger
{
    internal class AppSettings
    {
        private static AppSettings? instance;

        public static AppSettings GetInstance()
        {
            instance ??= new AppSettings();
            return instance;
        }

        private string? _logsPath;
        public string LogsPath
        {
            get { return _logsPath ?? ""; }
            set { _logsPath = value; }
        }

        private string? _loggingMode;
        public string LoggingMode
        {
            get { return _loggingMode ?? ""; }
            set { _loggingMode = value; }
        }

        internal AppSettings() {
        }
    }
}
