using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace LazyUp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly private AppConfigurator _appConfigurator = AppConfigurator.GetInstance();
        private AppSettings _appSettings = AppSettings.GetInstance();

        private TimeSpan _breakIntervals;
        private int _breakIntervalHoursTemp;
        private int _breakIntervalMinutesTemp;

        private TimeSpan _breakDuration;
        private int _breakDurationHoursTemp;
        private int _breakDurationMinutesTemp;

        readonly private System.Drawing.Icon _logoIcon;

        public MainWindow()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _logoIcon = new System.Drawing.Icon("Resources/logo.ico");
            }

            _appSettings = AppSettings.GetInstance();

            _breakIntervals = TimeSpan.FromSeconds(_appSettings.BreaksIntervalSec);

            _breakDuration = TimeSpan.FromSeconds(_appSettings.DurationBreakSec);

            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.Icon = Imaging.CreateBitmapSourceFromHIcon(
                _logoIcon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()); ;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_appSettings.CloseInTray)
            {
                e.Cancel = true;
                Visibility = Visibility.Hidden;
            }
        }

        static private string getValueTextBox(object sender)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            return text;
        }

        private int ConvertHoursAndMinutesIntoSeconds(int hours, int minutes)
        {
            return ((hours * 60 * 60) + (minutes * 60));
        }

        private void RunTestLockScreen_Button_Click(object sender, RoutedEventArgs e)
        {
            LockScreen lockWindow = new LockScreen();
            lockWindow.Show();
        }

        private void SaveSettings_Button_Click(object sender, RoutedEventArgs e)
        {
            _appConfigurator.UpdateConfig(ref _appSettings);
        }

        private void LockScreenHeader_Initialized(object sender, EventArgs e)
        {
            LockScreenHeader.Text = _appSettings.LockScreenHeader;
        }

        private void LockScreenHeaderText_TextChanged(object sender, TextChangedEventArgs e)
        {
            _appSettings.LockScreenHeader = getValueTextBox(sender);
        }

        private void LockScreenParagraph_Initialized(object sender, EventArgs e)
        {
            LockScreenParagraph.Text = _appSettings.LockScreenParagraph;
        }

        private void LockScreenParagraph_TextChanged(object sender, TextChangedEventArgs e)
        {
            _appSettings.LockScreenParagraph = getValueTextBox(sender);
        }

        private void radiobtnThemeDark_Initialized(object sender, EventArgs e)
        {
            radiobtnThemeDark.IsChecked = _appSettings.ThemeIsDark;
        }

        private void radiobtnThemeDark_Checked(object sender, RoutedEventArgs e)
        {
            _appSettings.ThemeIsDark = true;
        }

        private void radiobtnThemeLight_Initialized(object sender, EventArgs e)
        {
            radiobtnThemeLight.IsChecked = !_appSettings.ThemeIsDark;
        }

        private void radiobtnThemeLight_Checked(object sender, RoutedEventArgs e)
        {
            _appSettings.ThemeIsDark = false;
        }

        private void breaksIntervalHours_Initialized(object sender, EventArgs e)
        {
            _breakIntervalHoursTemp = _breakIntervals.Hours;
            breaksIntervalHours.Text = Convert.ToString(_breakIntervalHoursTemp);
        }

        private void breaksIntervalHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            _breakIntervalHoursTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void breaksIntervalHours_LostFocus(object sender, RoutedEventArgs e)
        {
            _appSettings.BreaksIntervalSec = ConvertHoursAndMinutesIntoSeconds(_breakIntervalHoursTemp, _breakIntervalMinutesTemp);
        }

        private void breaksIntervalMinutes_Initialized(object sender, EventArgs e)
        {
            _breakIntervalMinutesTemp = _breakIntervals.Minutes;
            breaksIntervalMinutes.Text = Convert.ToString(_breakIntervalMinutesTemp);
        }

        private void breaksIntervalMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            _breakIntervalMinutesTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void breaksIntervalMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            _appSettings.BreaksIntervalSec = ConvertHoursAndMinutesIntoSeconds(_breakIntervalHoursTemp, _breakIntervalMinutesTemp);
        }

        private void durationBreakHours_Initialized(object sender, EventArgs e)
        {
            
            _breakDurationHoursTemp = _breakDuration.Hours;
            durationBreakHours.Text = Convert.ToString(_breakDurationHoursTemp);
        }

        private void durationBreakHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            _breakDurationHoursTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void durationBreakHours_LostFocus(object sender, RoutedEventArgs e)
        {
            _appSettings.DurationBreakSec = ConvertHoursAndMinutesIntoSeconds(_breakDurationHoursTemp, _breakDurationMinutesTemp);
        }

        private void durationBreakMinutes_Initialized(object sender, EventArgs e)
        {
            _breakDurationMinutesTemp = _breakDuration.Minutes;
            durationBreakMinutes.Text = Convert.ToString(_breakDurationMinutesTemp);
        }

        private void durationBreakMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            _breakDurationMinutesTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void durationBreakMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            _appSettings.DurationBreakSec = ConvertHoursAndMinutesIntoSeconds(_breakDurationHoursTemp, _breakDurationMinutesTemp);
        }

        private void StartupWithSystemCheckBox_Initialized(object sender, EventArgs e)
        {
            StartupWithSystemCheckBox.IsChecked = _appSettings.StartupWithSystem;
        }

        private void StartupWithSystemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _appSettings.StartupWithSystem = true;
        }

        private void StartupWithSystemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _appSettings.StartupWithSystem = false;
        }

        private void CloseInTrayCheckBox_Initialized(object sender, EventArgs e)
        {
            CloseInTrayCheckBox.IsChecked = _appSettings.CloseInTray;
        }

        private void CloseInTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _appSettings.CloseInTray = true;
        }

        private void CloseInTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _appSettings.CloseInTray = false;

        }

        private void StartInTrayCheckBox_Initialized(object sender, EventArgs e)
        {
            StartInTrayCheckBox.IsChecked = _appSettings.StartInTray;
        }

        private void StartInTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _appSettings.StartInTray = true;
        }

        private void StartInTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _appSettings.StartInTray = false;
        }

        private void HideProgramCheckBox_Initialized(object sender, EventArgs e)
        {
            HideProgramCheckBox.IsChecked = _appSettings.HideProgram;
        }

        private void HideProgramCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _appSettings.HideProgram = true;
        }

        private void HideProgramCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _appSettings.HideProgram = false;
        }

        private void ReviveProgramCheckBox_Initialized(object sender, EventArgs e)
        {
            ReviveProgramCheckBox.IsChecked = _appSettings.ReviveProgram;
        }

        private void ReviveProgramCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _appSettings.ReviveProgram = true;
        }

        private void ReviveProgramCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _appSettings.ReviveProgram = false;
        }
    }
}
