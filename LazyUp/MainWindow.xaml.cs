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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Markup;

namespace LazyUp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppConfigurator configurator = AppConfigurator.GetInstance();
        private AppSettings config = AppConfigurator.GetInstance().config;

        private int breakIntervalHoursTemp;
        private int breakIntervalMinutesTemp;

        private int breakDurationHoursTemp;
        private int breakDurationMinutesTemp;

        private System.Drawing.Icon logoIcon = new System.Drawing.Icon("Resources/logo.ico");

        private string getValueTextBox(object sender)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            return text;
        }

        public MainWindow()
        {
            config = AppConfigurator.GetInstance().config;

            breakIntervalHoursTemp = config.BreaksIntervalSec / 60 / 60;
            breakIntervalMinutesTemp = (config.BreaksIntervalSec / 60) - (breakIntervalHoursTemp * 60);

            breakDurationHoursTemp = config.BreaksIntervalSec / 60 / 60;
            breakDurationMinutesTemp = (config.BreaksIntervalSec / 60) - (breakDurationHoursTemp * 60);

            InitializeComponent();
        }

        private void RunTestLockScreen_Button_Click(object sender, RoutedEventArgs e)
        {
            LockScreen lockWindow = new LockScreen();
            lockWindow.Show();
        }

        private void SaveSettings_Button_Click(object sender, RoutedEventArgs e)
        {
            configurator.UpdateConfig(config);
        }

        private void LockScreenHeader_Initialized(object sender, EventArgs e)
        {
            LockScreenHeader.Text = config.LockScreenHeader;
        }

        private void LockScreenHeaderText_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.LockScreenHeader = getValueTextBox(sender);
        }

        private void LockScreenParagraph_Initialized(object sender, EventArgs e)
        {
            LockScreenParagraph.Text = config.LockScreenParagraph;
        }

        private void LockScreenParagraph_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.LockScreenParagraph = getValueTextBox(sender);
        }

        private void radiobtnThemeDark_Initialized(object sender, EventArgs e)
        {
            radiobtnThemeDark.IsChecked = config.ThemeIsDark;
        }

        private void radiobtnThemeDark_Checked(object sender, RoutedEventArgs e)
        {
            config.ThemeIsDark = true;
        }

        private void radiobtnThemeLight_Initialized(object sender, EventArgs e)
        {
            radiobtnThemeLight.IsChecked = !config.ThemeIsDark;
        }

        private void radiobtnThemeLight_Checked(object sender, RoutedEventArgs e)
        {
            config.ThemeIsDark = false;
        }

        private void breaksIntervalHours_Initialized(object sender, EventArgs e)
        {
            breakIntervalHoursTemp = config.BreaksIntervalSec / 60 / 60;
            breaksIntervalHours.Text = Convert.ToString(breakIntervalHoursTemp);
        }

        private void breaksIntervalHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakIntervalHoursTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void breaksIntervalHours_LostFocus(object sender, RoutedEventArgs e)
        {
            config.BreaksIntervalSec = breakIntervalHoursTemp * 60 * 60 + breakIntervalMinutesTemp * 60;
        }

        private void breaksIntervalMinutes_Initialized(object sender, EventArgs e)
        {
            breakIntervalMinutesTemp = config.BreaksIntervalSec / 60 - breakIntervalHoursTemp * 60;
            breaksIntervalMinutes.Text = Convert.ToString(breakIntervalMinutesTemp);
        }

        private void breaksIntervalMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakIntervalMinutesTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void breaksIntervalMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            config.BreaksIntervalSec = breakIntervalHoursTemp * 60 * 60 + breakIntervalMinutesTemp * 60;
        }

        private void durationBreakHours_Initialized(object sender, EventArgs e)
        {
            breakDurationHoursTemp = config.DurationBreakSec / 60 / 60;
            durationBreakHours.Text = Convert.ToString(breakDurationHoursTemp);
        }

        private void durationBreakHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakDurationHoursTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void durationBreakHours_LostFocus(object sender, RoutedEventArgs e)
        {
            config.DurationBreakSec = breakDurationHoursTemp * 60 * 60 + breakDurationMinutesTemp * 60;
        }

        private void durationBreakMinutes_Initialized(object sender, EventArgs e)
        {
            breakDurationMinutesTemp = config.DurationBreakSec / 60 - breakDurationHoursTemp * 60;
            durationBreakMinutes.Text = Convert.ToString(breakDurationMinutesTemp);
        }

        private void durationBreakMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakDurationMinutesTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void durationBreakMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            config.DurationBreakSec = breakDurationHoursTemp * 60 * 60 + breakDurationMinutesTemp * 60;
        }

        private void StartupWithSystemCheckBox_Initialized(object sender, EventArgs e)
        {
            StartupWithSystemCheckBox.IsChecked = config.StartupWithSystem;
        }

        private void StartupWithSystemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            config.StartupWithSystem = true;
        }

        private void StartupWithSystemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            config.StartupWithSystem = false;
        }

        private void CloseInTrayCheckBox_Initialized(object sender, EventArgs e)
        {
            CloseInTrayCheckBox.IsChecked = config.CloseInTray;
        }

        private void CloseInTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            config.CloseInTray = true;
        }

        private void CloseInTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            config.CloseInTray = false;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(config.CloseInTray) { 
                e.Cancel = true;
                Visibility = Visibility.Hidden;
            }
        }

        private void StartInTrayCheckBox_Initialized(object sender, EventArgs e)
        {
            StartInTrayCheckBox.IsChecked = config.StartInTray;
        }

        private void StartInTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            config.StartInTray = true;
        }

        private void StartInTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            config.StartInTray = false;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.Icon = Imaging.CreateBitmapSourceFromHIcon(
            logoIcon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions()); ;
        }

        private void HideProgramCheckBox_Initialized(object sender, EventArgs e)
        {
            HideProgramCheckBox.IsChecked = config.HideProgram;
        }

        private void HideProgramCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            config.HideProgram = true;
        }

        private void HideProgramCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            config.HideProgram = false;
        }
    }
}
