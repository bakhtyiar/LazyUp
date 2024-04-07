using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace InputDeviceLogger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private FileSystemWatcher _configWatcher;
        readonly private AppConfigurator _configurator;
        readonly private AppSettings _config;

        App()
        {
            _configurator = AppConfigurator.GetInstance();
            _config = AppConfigurator.GetInstance().config;
        }

        private void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            _configurator.RefreshConfig();
        }

        private void SetUpConfigWatcher(ref FileSystemWatcher configWatcher)
        {
            string configFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App._config");
            configWatcher ??= new FileSystemWatcher(Path.GetDirectoryName(configFilePath) ?? @".\", Path.GetFileName(configFilePath));
            configWatcher.NotifyFilter = NotifyFilters.LastWrite;
            configWatcher.Changed += OnConfigChanged;
            configWatcher.EnableRaisingEvents = true;
        }

        private void SetRecordPath(string path)
        {
            
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SetUpConfigWatcher(ref _configWatcher);
            SetRecordPath(_config.LogsPath);
        }
    }
}
