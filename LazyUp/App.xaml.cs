using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Timers;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace LazyUp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string breaksIntervalSec = ConfigurationManager.AppSettings["breaksIntervalSec"] ?? "3600";
        string durationBreakSec = ConfigurationManager.AppSettings["durationBreakSec"] ?? "600";
        string startupWithSystem = ConfigurationManager.AppSettings["startupWithSystem"] ?? "true";
        string startInTray = ConfigurationManager.AppSettings["startInTray"] ?? "true";
        string closeInTray = ConfigurationManager.AppSettings["closeInTray"] ?? "true";
        string shutdownModeValue;

        private FileSystemWatcher _configWatcher;
        private Timer timerBreakInterval;
        private MainWindow mainWindow = new MainWindow();
        private Forms.NotifyIcon _notifyIcon;
        private System.Drawing.Icon logoIcon = new System.Drawing.Icon("Resources/logo.ico");
        private System.Drawing.Icon openIcon = new System.Drawing.Icon("Resources/open.ico");
        private System.Drawing.Icon closeIcon = new System.Drawing.Icon("Resources/close.ico");

        App()
        {
            _notifyIcon = new Forms.NotifyIcon();
            _notifyIcon.Icon = logoIcon;

            shutdownModeValue = (closeInTray ?? "true") == "true" ? "OnExplicitShutdown" : "OnMainWindowClose";
            if (Enum.TryParse<ShutdownMode>(shutdownModeValue, out ShutdownMode shutdownMode))
            {
                Current.ShutdownMode = shutdownMode;
            }
            else
            {
                throw new Exception("Config error. No shutdownMode value with key closeInTray");
            }

            timerBreakInterval = new System.Timers.Timer();
            timerBreakInterval.Interval = (Convert.ToInt32(breaksIntervalSec) + Convert.ToInt32(durationBreakSec)) * 1000;
            timerBreakInterval.Elapsed += startBreak;
            timerBreakInterval.AutoReset = true;
            timerBreakInterval.Enabled = true;
        }

        private void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            ConfigurationManager.RefreshSection("appSettings");
            startupWithSystem = ConfigurationManager.AppSettings["startupWithSystem"] ?? "true";
            if (startupWithSystem == "true")
            {
                string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                RegistryKey key = Registry.LocalMachine.OpenSubKey(path, true);
                key.SetValue("LazyUp", System.Reflection.Assembly.GetExecutingAssembly().Location.ToString());
            } else
            {
                string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                key.DeleteValue("LazyUp", false);
            }

        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            mainWindow = mainWindow ?? new MainWindow();
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Show();
            mainWindow.Activate();
        }

        private void startBreak(Object source, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LockScreen lockWindow = new LockScreen();
                lockWindow.Show();
            });
        }

        private void NotifyIconOnClickedOpen(object sender, EventArgs e)
        {
            NotifyIcon_DoubleClick(sender, e);
        }
        private void NotifyIconOnClickedClose(object sender, EventArgs e)
        {
            Current.Shutdown();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if(startInTray == "false")
            {
                this.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative);
            }

            string configFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App.config");
            _configWatcher = new FileSystemWatcher(Path.GetDirectoryName(configFilePath), Path.GetFileName(configFilePath));
            _configWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _configWatcher.Changed += OnConfigChanged;
            _configWatcher.EnableRaisingEvents = true;

            _notifyIcon.Visible = true;
            _notifyIcon.Text = "LazyUp";
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Open", openIcon.ToBitmap(), NotifyIconOnClickedOpen);
            _notifyIcon.ContextMenuStrip.Items.Add("Close", closeIcon.ToBitmap(), NotifyIconOnClickedClose);

            // base.OnStartup(e);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _notifyIcon.Dispose();

            // base.OnExit(e);
        }
    }
}
