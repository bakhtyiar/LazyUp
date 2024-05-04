using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using Gma.System.MouseKeyHook;

namespace InputDeviceLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string dontDisturb = "DontDisturb";
        private const string disturb = "Disturb";
        readonly private AppConfigurator _appConfigurator = AppConfigurator.GetInstance();
        static private AppSettings _appSettings = AppSettings.GetInstance();
        static string logFileName = $"{_appSettings.LoggingMode}_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.txt";
        static string logPath = System.IO.Path.Combine(_appSettings.LogsPath, logFileName);
        private static IKeyboardMouseEvents m_GlobalHook;

        public MainWindow()
        {
            InitializeComponent();
        }



        static private string getValueTextBox(object sender)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            return text;
        }

        private void DontDisturbRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _appConfigurator.UpdateSetting("LoggingMode", dontDisturb);
            _appSettings.LoggingMode = dontDisturb;
        }

        private void DisturbRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _appConfigurator.UpdateSetting("LoggingMode", disturb);
            _appSettings.LoggingMode = disturb;
        }

        private void StartRecordButton_Click(object sender, RoutedEventArgs e)
        {
            btn_startrec.IsEnabled = false;
            btn_stoprec.IsEnabled = true;
            /*backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;*/
            if (!Directory.Exists(_appSettings.LogsPath))
            {
                Directory.CreateDirectory(_appSettings.LogsPath);
            }
            logFileName = $"{_appSettings.LoggingMode}_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.txt";
            logPath = System.IO.Path.Combine(_appSettings.LogsPath, logFileName);
            if (!File.Exists(logPath))
            {
                using (FileStream fs = File.Create(logPath))
                {
                    Console.WriteLine("Файл создан: " + logPath);
                }
            }
            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.KeyDown += GlobalHookKeyDown;
            m_GlobalHook.KeyUp+= GlobalHookKeyUp;

            m_GlobalHook.MouseDownExt += GlobalHookMouseDown;
            m_GlobalHook.MouseUpExt += GlobalHookMouseUp;


            using (StreamWriter writer = File.AppendText(logPath))
            {
                Console.WriteLine("Начало записи...");
                PreviewKeyDown += MainWindow_PreviewKeyDown;
                PreviewKeyUp+= MainWindow_PreviewKeyUp;
                MouseWheel += MainWindow_MouseWheel;
                MouseDown += MainWindow_MouseDown;
                MouseUp += MainWindow_MouseUp;
                Console.WriteLine($"Лог записан в файл: {logPath}");
            }
        }
       
        private void StopRecordButton_Click(object sender, RoutedEventArgs e)
        {
            btn_startrec.IsEnabled = true;
            btn_stoprec.IsEnabled = false;
            m_GlobalHook.Dispose();
        }

        private static void GlobalHookKeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            using (StreamWriter writer = File.AppendText(logPath))
            {
                writer.WriteLine($"[{DateTime.Now}][Keyboard][Down][{e.KeyCode}]");
            }
        }
        private static void GlobalHookKeyUp(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            using (StreamWriter writer = File.AppendText(logPath))
            {
                writer.WriteLine($"[{DateTime.Now}][Keyboard][Up][{e.KeyCode}]");
            }
        }

        private static void GlobalHookMouseDown(object sender, MouseEventExtArgs e)
        {
            using (StreamWriter writer = File.AppendText(logPath))
            {
                writer.WriteLine($"[{DateTime.Now}][Mouse][Down][{e.Button}]");
            }
        }
        private static void GlobalHookMouseUp(object sender, MouseEventExtArgs e)
        {
            using (StreamWriter writer = File.AppendText(logPath))
            {
                writer.WriteLine($"[{DateTime.Now}][Mouse][Down][{e.Button}]");
            }
        }
        private void AnalyzeLogsButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            using (StreamWriter writer = File.AppendText(logPath))
            {
                writer.WriteLine($"[{DateTime.Now}][Keyboard][Down][{e.Key}]");
            }
        }
        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            using (StreamWriter writer = File.AppendText(logPath))
            {
                writer.WriteLine($"[{DateTime.Now}][Keyboard][Up][{e.Key}]");
            }
        }
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            using (StreamWriter writer = File.AppendText(logPath))
            {
                writer.WriteLine($"[{DateTime.Now}][Mouse][Down][{e.ChangedButton}]");
            }
        }
        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            using (StreamWriter writer = File.AppendText(logPath))
            {
                writer.WriteLine($"[{DateTime.Now}][Mouse][Up][{e.ChangedButton}]");
            }
        }
        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            using (StreamWriter writer = File.AppendText(logPath))
            {
                writer.WriteLine($"[{DateTime.Now}][Mouse][Wheel][{e.Delta}]");
            }
        }

        private void Radiobtn_DisturbMe_Initialized(object sender, EventArgs e)
        {
            Radiobtn_DisturbMe.IsChecked = _appSettings.LoggingMode == disturb;
        }

        private void Radiobtn_DontDisturbMe_Initialized(object sender, EventArgs e)
        {
            Radiobtn_DontDisturbMe.IsChecked = _appSettings.LoggingMode == dontDisturb;
        }
    }
}
