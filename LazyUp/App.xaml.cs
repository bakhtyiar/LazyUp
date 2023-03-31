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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using IWshRuntimeLibrary;

namespace LazyUp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        int breaksIntervalSec = Convert.ToInt32(ConfigurationManager.AppSettings["breaksIntervalSec"]);
        int breaksDurationSec = Convert.ToInt32(ConfigurationManager.AppSettings["durationBreakSec"]);
        bool startupWithSystem = Convert.ToBoolean(ConfigurationManager.AppSettings["startupWithSystem"]);
        bool startInTray = Convert.ToBoolean(ConfigurationManager.AppSettings["startInTray"]);
        bool closeInTray = Convert.ToBoolean(ConfigurationManager.AppSettings["closeInTray"]);
        bool hideProgram = Convert.ToBoolean(ConfigurationManager.AppSettings["hideProgram"]);
        string shutdownModeValue;

        private FileSystemWatcher _configWatcher;
        private System.Timers.Timer timerBreakInterval;
        private MainWindow mainWindow = new MainWindow();
        private Forms.NotifyIcon _notifyIcon;
        private System.Drawing.Icon logoIcon = new System.Drawing.Icon(@"Resources\logo.ico");
        private System.Drawing.Icon openIcon = new System.Drawing.Icon(@"Resources\open.ico");
        private System.Drawing.Icon closeIcon = new System.Drawing.Icon(@"Resources\close.ico");
        private string execDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();
        private string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        private string programBrandName = "LazyUp";
        private string programPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString() + @"\LazyUp.exe";

        App()
        {
            
        }

        private void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            ConfigurationManager.RefreshSection("appSettings");

            startupWithSystem = Convert.ToBoolean(ConfigurationManager.AppSettings["startupWithSystem"]);
            SetStartupProgram(startupWithSystem);

            hideProgram = Convert.ToBoolean(ConfigurationManager.AppSettings["hideProgram"]);
            SetProgramVisibility(hideProgram);

            closeInTray = Convert.ToBoolean(ConfigurationManager.AppSettings["closeInTray"]);
            SetCloseInTray(closeInTray);
        }

        private void SetCloseInTray(bool isCloseInTray)
        {
            shutdownModeValue = isCloseInTray ? "OnExplicitShutdown" : "OnMainWindowClose";
            if (Enum.TryParse<ShutdownMode>(shutdownModeValue, out ShutdownMode shutdownMode))
            {
                Current.ShutdownMode = shutdownMode;
            }
            else
            {
                throw new Exception("Config error. No shutdownMode value with key closeInTray");
            }
        }

        private void SetProgramVisibility(bool isHiddenProgram)
        {
            if (isHiddenProgram)
            {
                _notifyIcon.Visible = false;
            } else
            {
                _notifyIcon.Visible = true;
            }
        }

        private void SetStartupProgram(bool isStartup) {
            string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            if (isStartup)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                key.SetValue(programBrandName, programPath);

                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Path.Combine(startupFolderPath, programBrandName + ".lnk"));

                shortcut.TargetPath = programPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(programPath);
                shortcut.Description = programBrandName;
                shortcut.IconLocation = Path.Combine(execDirectory, @"Resources\logo.ico");
                shortcut.Save();
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                key.DeleteValue(programBrandName, false);

                string shortcutPath = Path.Combine(startupFolderPath, programBrandName + ".lnk");
                if (System.IO.File.Exists(shortcutPath))
                {
                    System.IO.File.Delete(shortcutPath);
                }
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
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
            _notifyIcon = new Forms.NotifyIcon();
            _notifyIcon.Icon = logoIcon;

            timerBreakInterval = new System.Timers.Timer();
            timerBreakInterval.Interval = (breaksIntervalSec + breaksDurationSec) * 1000;
            timerBreakInterval.Elapsed += startBreak;
            timerBreakInterval.AutoReset = true;
            timerBreakInterval.Enabled = true;

            if (!startInTray)
            {
                //this.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative);
                mainWindow = mainWindow ?? new MainWindow();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Show();
                mainWindow.Activate();
            }

            SetStartupProgram(startupWithSystem);

            SetCloseInTray(closeInTray);

            string configFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App.config");
            _configWatcher = new FileSystemWatcher(Path.GetDirectoryName(configFilePath), Path.GetFileName(configFilePath));
            _configWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _configWatcher.Changed += OnConfigChanged;
            _configWatcher.EnableRaisingEvents = true;

            _notifyIcon.Text = programBrandName;
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Open", openIcon.ToBitmap(), NotifyIconOnClickedOpen);
            _notifyIcon.ContextMenuStrip.Items.Add("Close", closeIcon.ToBitmap(), NotifyIconOnClickedClose);
            SetProgramVisibility(hideProgram);

            // base.OnStartup(e);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _notifyIcon = null;

            // base.OnExit(e);
        }
    }
}
