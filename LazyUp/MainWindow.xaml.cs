using System;
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
using System.Configuration;
using System.Reflection;
using System.Drawing;

namespace LazyUp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string lockScreenHeader = ConfigurationManager.AppSettings["lockScreenHeader"] ?? "Go move";
        string lockScreenParagraph = ConfigurationManager.AppSettings["lockScreenParagraph"] ?? "Regular activity makes you healthy and happy";
        string themeIsDark = ConfigurationManager.AppSettings["themeIsDark"] ?? "true";
        string breaksIntervalSec = ConfigurationManager.AppSettings["breaksIntervalSec"] ?? "3600";
        string durationBreakSec = ConfigurationManager.AppSettings["durationBreakSec"] ?? "600";
        string strictBreaks = ConfigurationManager.AppSettings["strictBreaks"] ?? "true";
        string lookHiddenRest = ConfigurationManager.AppSettings["lookHiddenRest"] ?? "true";
        string startupWithSystem = ConfigurationManager.AppSettings["startupWithSystem"] ??  "true";
        string startInTray = ConfigurationManager.AppSettings["startInTray"] ?? "true";
        string closeInTray = ConfigurationManager.AppSettings["closeInTray"] ?? "true";

        int breakIntervalHoursTemp;
        int breakIntervalMinutesTemp;

        int breakDurationHoursTemp;
        int breakDurationMinutesTemp;

        // public static string BaseDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


        private string getValueTextBox(object sender)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            return text;
        }

        private static void UpdateSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }

        public MainWindow()
        {
            breakIntervalHoursTemp = Convert.ToInt32(breaksIntervalSec) / 60 / 60;
            breakIntervalMinutesTemp = (Convert.ToInt32(breaksIntervalSec) / 60) - (Convert.ToInt32(breakIntervalHoursTemp) * 60);

            breakDurationHoursTemp = Convert.ToInt32(breaksIntervalSec) / 60 / 60;
            breakDurationMinutesTemp = (Convert.ToInt32(breaksIntervalSec) / 60) - (Convert.ToInt32(breakDurationHoursTemp) * 60);

            InitializeComponent();
        }

        private void RunTestLockScreen_Button_Click(object sender, RoutedEventArgs e)
        {
            LockScreen lockWindow = new LockScreen();
            lockWindow.Show();
        }

        private void SaveSettings_Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateSetting("lockScreenHeader", lockScreenHeader);
            UpdateSetting("lockScreenParagraph", lockScreenParagraph);
            UpdateSetting("themeIsDark", themeIsDark);
            UpdateSetting("breaksIntervalSec", breaksIntervalSec);
            UpdateSetting("durationBreakSec", durationBreakSec);
            UpdateSetting("strictBreaks", strictBreaks);
            UpdateSetting("lookHiddenRest", lookHiddenRest);
            UpdateSetting("startupWithSystem", startupWithSystem);
            UpdateSetting("startInTray", startInTray);
            UpdateSetting("closeInTray", closeInTray);
        }

        private void LockScreenHeader_Initialized(object sender, EventArgs e)
        {
            LockScreenHeader.Text = lockScreenHeader;
        }

        private void LockScreenHeaderText_TextChanged(object sender, TextChangedEventArgs e)
        {
            lockScreenHeader = getValueTextBox(sender);
        }

        private void LockScreenParagraph_Initialized(object sender, EventArgs e)
        {
            LockScreenParagraph.Text = lockScreenParagraph;
        }

        private void LockScreenParagraph_TextChanged(object sender, TextChangedEventArgs e)
        {
            lockScreenParagraph = getValueTextBox(sender);
        }

        private void radiobtnThemeDark_Initialized(object sender, EventArgs e)
        {
            radiobtnThemeDark.IsChecked = themeIsDark == "true";
        }

        private void radiobtnThemeDark_Checked(object sender, RoutedEventArgs e)
        {
            themeIsDark = "true";
        }

        private void radiobtnThemeLight_Initialized(object sender, EventArgs e)
        {
            radiobtnThemeLight.IsChecked = themeIsDark == "false";
        }

        private void radiobtnThemeLight_Checked(object sender, RoutedEventArgs e)
        {
            themeIsDark = "false";
        }

        private void breaksIntervalHours_Initialized(object sender, EventArgs e)
        {
            breakIntervalHoursTemp = Convert.ToInt32(breaksIntervalSec) / 60 / 60;
            breaksIntervalHours.Text = Convert.ToString(breakIntervalHoursTemp);
        }

        private void breaksIntervalHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakIntervalHoursTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void breaksIntervalHours_LostFocus(object sender, RoutedEventArgs e)
        {
            breaksIntervalSec = Convert.ToString(breakIntervalHoursTemp * 60 * 60 + breakIntervalMinutesTemp * 60);
        }

        private void breaksIntervalMinutes_Initialized(object sender, EventArgs e)
        {
            breakIntervalMinutesTemp = Convert.ToInt32(breaksIntervalSec) / 60 - breakIntervalHoursTemp * 60;
            breaksIntervalMinutes.Text = Convert.ToString(breakIntervalMinutesTemp);
        }

        private void breaksIntervalMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakIntervalMinutesTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void breaksIntervalMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            breaksIntervalSec = Convert.ToString(breakIntervalHoursTemp * 60 * 60 + breakIntervalMinutesTemp * 60);
        }

        private void durationBreakHours_Initialized(object sender, EventArgs e)
        {
            breakDurationHoursTemp = Convert.ToInt32(durationBreakSec) / 60 / 60;
            durationBreakHours.Text = Convert.ToString(breakDurationHoursTemp);
        }

        private void durationBreakHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakDurationHoursTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void durationBreakHours_LostFocus(object sender, RoutedEventArgs e)
        {
            durationBreakSec = Convert.ToString(breakDurationHoursTemp * 60 * 60 + breakDurationMinutesTemp * 60);
        }

        private void durationBreakMinutes_Initialized(object sender, EventArgs e)
        {
            breakDurationMinutesTemp = Convert.ToInt32(durationBreakSec) / 60 - breakDurationHoursTemp * 60;
            durationBreakMinutes.Text = Convert.ToString(breakDurationMinutesTemp);
        }

        private void durationBreakMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakDurationMinutesTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void durationBreakMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            durationBreakSec = Convert.ToString(breakDurationHoursTemp * 60 * 60 + breakDurationMinutesTemp * 60);
        }

        private void StartupWithSystemCheckBox_Initialized(object sender, EventArgs e)
        {
            StartupWithSystemCheckBox.IsChecked = startupWithSystem == "true";
        }

        private void StartupWithSystemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
           startupWithSystem = "true";
        }

        private void StartupWithSystemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            startupWithSystem = "false";
        }

        private void CloseInTrayCheckBox_Initialized(object sender, EventArgs e)
        {
            CloseInTrayCheckBox.IsChecked = startupWithSystem == "true";
        }

        private void CloseInTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            closeInTray = "true";
        }

        private void CloseInTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            closeInTray = "false";

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void StartInTrayCheckBox_Initialized(object sender, EventArgs e)
        {
            StartInTrayCheckBox.IsChecked = startInTray == "true";
        }

        private void StartInTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            startInTray = "true";
        }

        private void StartInTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            startInTray = "false";
        }
    }
}
