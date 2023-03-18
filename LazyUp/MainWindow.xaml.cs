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
        string lockScreenHeader = ConfigurationManager.AppSettings["lockScreenHeader"];
        string lockScreenParagraph = ConfigurationManager.AppSettings["lockScreenParagraph"];
        string lockScreenBackgroundColor = ConfigurationManager.AppSettings["lockScreenBackgroundColor"];
        string breaksIntervalSec = ConfigurationManager.AppSettings["breaksIntervalSec"];
        string durationBreakSec = ConfigurationManager.AppSettings["durationBreakSec"];
        string strictBreaks = ConfigurationManager.AppSettings["strictBreaks"];
        string lookHiddenRest = ConfigurationManager.AppSettings["lookHiddenRest"];
        string startupWithSystem = ConfigurationManager.AppSettings["startupWithSystem"];
        string startInTray = ConfigurationManager.AppSettings["startInTray"];

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
            UpdateSetting("lockScreenBackgroundColor", lockScreenBackgroundColor);
            UpdateSetting("breaksIntervalSec", breaksIntervalSec);
            UpdateSetting("durationBreakSec", durationBreakSec);
            UpdateSetting("strictBreaks", strictBreaks);
            UpdateSetting("lookHiddenRest", lookHiddenRest);
            UpdateSetting("startupWithSystem", startupWithSystem);
            UpdateSetting("startInTray", startInTray);
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

        private void radiobtnThemeDark_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void radiobtnThemeLight_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
