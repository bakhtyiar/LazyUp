using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows;
using Forms = System.Windows.Forms;

namespace LazyUp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        readonly Process _process;

        private MainWindow _mainWindow;

        private FileSystemWatcher _configWatcher;
        readonly private AppConfigurator _configurator;
        readonly private AppSettings _config;

        private System.Timers.Timer _timerBreakInterval;
        string _shutdownModeValue;

        readonly private string _execDirectory;
        readonly private string _startupFolderPath;
        readonly private string _programBrandName;
        readonly private string _programPath;
        readonly private string _agentReviverProgramPath;

        private Forms.NotifyIcon _notifyIcon;
        readonly private System.Drawing.Icon _logoIcon;
        readonly private System.Drawing.Icon _openIcon;
        readonly private System.Drawing.Icon _closeIcon;

        App()
        {
            _process = new Process();

            _configurator = AppConfigurator.GetInstance();
            _config = AppConfigurator.GetInstance().config;

            _mainWindow = new MainWindow();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _logoIcon = new System.Drawing.Icon(@"Resources\logo.ico");
                _openIcon = new System.Drawing.Icon(@"Resources\open.ico");
                _closeIcon = new System.Drawing.Icon(@"Resources\close.ico");
            }

            _execDirectory = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? @".\").ToString();
            _startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            _programBrandName = "LazyUp";
            _programPath = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".").ToString() + @"\LazyUp.exe";
            _agentReviverProgramPath = (Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? @".").ToString() + @"\AgentOfLazyUp.exe";
        }

        private void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            _configurator.RefreshConfig();
            SetStartupProgram(_config.StartupWithSystem);
            SetProgramVisibility(ref _notifyIcon, _config.HideProgram);
            SetCloseInTray(out _shutdownModeValue, _config.CloseInTray);
            ReviveAgentOfLazyUp(_config.ReviveProgram);
        }

        static private void SetCloseInTray(out string shutdownModeValue, bool isCloseInTray)
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

        static private void SetProgramVisibility(ref Forms.NotifyIcon notifyIcon, bool isHiddenProgram)
        {
            notifyIcon.Visible = !isHiddenProgram;
        }

        private void SetStartupProgram(bool isStartup)
        {
            string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            if (isStartup)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    RegistryKey currentUser = Registry.CurrentUser;
                    RegistryKey? key = currentUser.OpenSubKey(path, true);
                    key?.SetValue(_programBrandName, _programPath);
                }

                string linksPath = Path.Combine(_startupFolderPath, _programBrandName + ".lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(linksPath);

                shortcut.TargetPath = _programPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(_programPath);
                shortcut.Description = _programBrandName;
                shortcut.IconLocation = Path.Combine(_execDirectory, @"Resources\logo.ico");
                shortcut.Save();
            }
            else
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    RegistryKey? key = Registry.CurrentUser.OpenSubKey(path, true);
                    key?.DeleteValue(_programBrandName, false);
                }

                string shortcutPath = Path.Combine(_startupFolderPath, _programBrandName + ".lnk");
                if (System.IO.File.Exists(shortcutPath))
                {
                    System.IO.File.Delete(shortcutPath);
                }
            }
        }

        private void StartBreak(Object? source, System.Timers.ElapsedEventArgs e)
        {
            Current.Dispatcher.Invoke(() =>
            {
                LockScreen lockWindow = new LockScreen();
                lockWindow.Show();
            });
        }

        private void NotifyIcon_OpenApp(object? sender, EventArgs e)
        {
            _mainWindow ??= new MainWindow();
            _mainWindow.WindowState = System.Windows.WindowState.Normal;
            _mainWindow.Show();
            _mainWindow.Activate();
        }

        private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            NotifyIcon_OpenApp(sender, e);
        }

        private void NotifyIconOnClickedOpen(object? sender, EventArgs e)
        {
            NotifyIcon_OpenApp(sender, e);
        }

        private void NotifyIconOnClickedClose(object? sender, EventArgs e)
        {
            /*Current.Shutdown();*/
            Environment.Exit(0);
        }

        static private void ReviveAgentOfLazyUp(bool reviveProgram, out Thread thread)
        {
            // Имя программы, которую нужно проверить
            string processName = "AgentOfLazyUp";
            CancellationTokenSource cts = new CancellationTokenSource();
            thread = new Thread(() => CheckAndStartProcess(cts.Token));

            if (reviveProgram)
            {
                // Запускаем проверку в фоновом потоке
                thread.Start();
            }
            else
            {
                cts.Cancel(); // Останавливаем поток

                // Ищем процесс по имени
                Process[] foundProcesses = Process.GetProcessesByName(processName);

                // Если процесс найден, останавливаем его
                if (foundProcesses.Length > 0)
                {
                    for (int i = 0; i < foundProcesses.Length; i++)
                    {
                        foundProcesses[i].Kill();
                    }
                }
            }

            void CheckAndStartProcess(CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    Process[] foundProcesses = Process.GetProcessesByName(processName);
                    // Проверяем, запущена ли программа
                    if (foundProcesses.Length < 1)
                    {
                        // Программа не запущена, запускаем ее
                        Process process = Process.Start(processName + ".exe");
                        process.WaitForExit();
                    } /*else if (foundProcesses.Length > 1)
                    {
                        for (int i = 0; i < foundProcesses.Length; i++)
                        {
                            foundProcesses[i].Kill();
                        }
                    }*/

                    // Ждем перед следующей проверкой
                    Thread.Sleep(5000);
                }
            }
        }

        static private void ReviveAgentOfLazyUp(bool reviveProgram)
        {
            // Имя программы, которую нужно проверить
            string processName = "AgentOfLazyUp";
            CancellationTokenSource cts = new CancellationTokenSource();
            Thread thread = new Thread(() => CheckAndStartProcess(cts.Token));

            if (reviveProgram)
            {
                // Запускаем проверку в фоновом потоке
                thread.Start();
            }
            else
            {
                cts.Cancel(); // Останавливаем поток

                // Ищем процесс по имени
                Process[] foundProcesses = Process.GetProcessesByName(processName);

                // Если процесс найден, останавливаем его
                if (foundProcesses.Length > 0)
                {
                    for (int i = 0; i < foundProcesses.Length; i++)
                    {
                        foundProcesses[i].Kill();
                    }
                }
            }

            void CheckAndStartProcess(CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    Process[] foundProcesses = Process.GetProcessesByName(processName);
                    // Проверяем, запущена ли программа
                    if (foundProcesses.Length < 1)
                    {
                        // Программа не запущена, запускаем ее
                        Process process = Process.Start(processName + ".exe");
                        process.WaitForExit();
                    } /*else if (foundProcesses.Length > 1)
                    {
                        for (int i = 0; i < foundProcesses.Length; i++)
                        {
                            foundProcesses[i].Kill();
                        }
                    }*/

                    // Ждем перед следующей проверкой
                    Thread.Sleep(5000);
                }
            }
        }

        delegate void StartBreakDelegate(Object? source, System.Timers.ElapsedEventArgs e);
        static private void SetUpBreaksTimer(ref System.Timers.Timer timerBreakInterval, int intervalSec, int durationSec, StartBreakDelegate startBreak)
        {
            timerBreakInterval ??= new System.Timers.Timer();
            timerBreakInterval.Interval = (intervalSec + durationSec) * 1000;
            timerBreakInterval.Elapsed += new ElapsedEventHandler(startBreak);
            timerBreakInterval.AutoReset = true;
            timerBreakInterval.Enabled = true;
        }

        static private void SetUpStartingInTray(ref MainWindow mainWindow, bool StartInTray)
        {
            mainWindow ??= new MainWindow();
            if (!StartInTray)
            {
                mainWindow.WindowState = System.Windows.WindowState.Normal;
                mainWindow.Show();
                mainWindow.Activate();
            }
        }

        private void SetUpConfigWatcher(ref FileSystemWatcher configWatcher)
        {
            string configFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App._config");
            configWatcher ??= new FileSystemWatcher(Path.GetDirectoryName(configFilePath) ?? @".\", Path.GetFileName(configFilePath));
            configWatcher.NotifyFilter = NotifyFilters.LastWrite;
            configWatcher.Changed += OnConfigChanged;
            configWatcher.EnableRaisingEvents = true;
        }

        private void SetUpNotifyIcon(ref Forms.NotifyIcon notifyIcon)
        {
            notifyIcon ??= new Forms.NotifyIcon();
            notifyIcon.Text = _programBrandName;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
        }
        private void SetUpNotifyIcon(ref Forms.NotifyIcon notifyIcon, ref Dictionary<string, System.Drawing.Icon> icons)
        {
            notifyIcon ??= new Forms.NotifyIcon();
            notifyIcon.Text = _programBrandName;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                notifyIcon.Icon = icons["logoIcon"];
                notifyIcon.ContextMenuStrip.Items.Add("Open", icons["openIcon"].ToBitmap(), NotifyIconOnClickedOpen);
                notifyIcon.ContextMenuStrip.Items.Add("Close", icons["closeIcon"].ToBitmap(), NotifyIconOnClickedClose);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SetStartupProgram(_config.StartupWithSystem);
            SetUpStartingInTray(ref _mainWindow, _config.StartInTray);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Dictionary<string, System.Drawing.Icon> iconsPack = new Dictionary<string, System.Drawing.Icon>
                {
                    { "logoIcon", _logoIcon },
                    { "openIcon", _openIcon },
                    { "closeIcon", _closeIcon }
                };
                SetUpNotifyIcon(ref _notifyIcon, ref iconsPack);
            }
            else
            {
                SetUpNotifyIcon(ref _notifyIcon);
            }
            SetProgramVisibility(ref _notifyIcon, _config.HideProgram);

            SetUpBreaksTimer(ref _timerBreakInterval, _config.BreaksIntervalSec, _config.DurationBreakSec, StartBreak);
            SetCloseInTray(out _shutdownModeValue, _config.CloseInTray);
            SetUpConfigWatcher(ref _configWatcher);
            ReviveAgentOfLazyUp(_config.ReviveProgram);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
    }
}
