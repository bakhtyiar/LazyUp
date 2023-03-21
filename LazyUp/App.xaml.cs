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
        private System.Drawing.Icon logoIcon = new System.Drawing.Icon("Resources/logo.ico");
        private System.Drawing.Icon openIcon = new System.Drawing.Icon("Resources/open.ico");
        private System.Drawing.Icon closeIcon = new System.Drawing.Icon("Resources/close.ico");

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private void HideFromTaskManager(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, new IntPtr((int)exStyle | WS_EX_TOOLWINDOW));
        }

        App()
        {
            
        }

        private void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            ConfigurationManager.RefreshSection("appSettings");

            startupWithSystem = Convert.ToBoolean(ConfigurationManager.AppSettings["startupWithSystem"]);
            SetStartupProgram(startInTray);

            hideProgram = Convert.ToBoolean(ConfigurationManager.AppSettings["hideProgram"]);
            SetProgramVisibility(hideProgram);
        }

        private void SetProgramVisibility(bool isHiddenProgram)
        {
            if (isHiddenProgram)
            {
                _notifyIcon.Visible = false;
                HideFromTaskManager(mainWindow);
            } else
            {
                _notifyIcon.Visible = true;
            }
        }

        private void SetStartupProgram(bool isStartup) {
            if (isStartup)
            {
                string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                key.SetValue("LazyUp", System.Reflection.Assembly.GetExecutingAssembly().Location.ToString());
            }
            else
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
                this.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative);
            }

            SetStartupProgram(startupWithSystem);

            string configFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App.config");
            _configWatcher = new FileSystemWatcher(Path.GetDirectoryName(configFilePath), Path.GetFileName(configFilePath));
            _configWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _configWatcher.Changed += OnConfigChanged;
            _configWatcher.EnableRaisingEvents = true;

            SetProgramVisibility(hideProgram);
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
