﻿using System;
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

namespace LazyUp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string lockScreenHeader = ConfigurationManager.AppSettings["lockScreenHeader"] ?? "Stroll out";
        string lockScreenParagraph = ConfigurationManager.AppSettings["lockScreenParagraph"] ?? "Regular activity makes you live longer";
        bool themeIsDark = Convert.ToBoolean(ConfigurationManager.AppSettings["themeIsDark"]);
        int breaksIntervalSec = Convert.ToInt32(ConfigurationManager.AppSettings["breaksIntervalSec"]);
        int durationBreakSec = Convert.ToInt32(ConfigurationManager.AppSettings["durationBreakSec"]);
        bool strictBreaks = Convert.ToBoolean(ConfigurationManager.AppSettings["strictBreaks"]);
        bool lookHiddenRest = Convert.ToBoolean(ConfigurationManager.AppSettings["lookHiddenRest"]);
        bool startupWithSystem = Convert.ToBoolean(ConfigurationManager.AppSettings["startupWithSystem"]);
        bool startInTray = Convert.ToBoolean(ConfigurationManager.AppSettings["startInTray"]);
        bool closeInTray = Convert.ToBoolean(ConfigurationManager.AppSettings["closeInTray"]);
        bool hideProgram = Convert.ToBoolean(ConfigurationManager.AppSettings["hideProgram"]);

        int breakIntervalHoursTemp;
        int breakIntervalMinutesTemp;

        int breakDurationHoursTemp;
        int breakDurationMinutesTemp;

        private System.Drawing.Icon logoIcon = new System.Drawing.Icon("Resources/logo.ico");

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
            breakIntervalHoursTemp = breaksIntervalSec / 60 / 60;
            breakIntervalMinutesTemp = (breaksIntervalSec / 60) - (breakIntervalHoursTemp * 60);

            breakDurationHoursTemp = breaksIntervalSec / 60 / 60;
            breakDurationMinutesTemp = (breaksIntervalSec / 60) - (breakDurationHoursTemp * 60);

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
            UpdateSetting("themeIsDark", themeIsDark.ToString().ToLower());
            UpdateSetting("breaksIntervalSec", breaksIntervalSec.ToString());
            UpdateSetting("durationBreakSec", durationBreakSec.ToString());
            UpdateSetting("strictBreaks", strictBreaks.ToString().ToLower());
            UpdateSetting("lookHiddenRest", lookHiddenRest.ToString().ToLower());
            UpdateSetting("startupWithSystem", startupWithSystem.ToString().ToLower());
            UpdateSetting("startInTray", startInTray.ToString().ToLower());
            UpdateSetting("closeInTray", closeInTray.ToString().ToLower());
            UpdateSetting("hideProgram", hideProgram.ToString().ToLower());
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
            radiobtnThemeDark.IsChecked = themeIsDark;
        }

        private void radiobtnThemeDark_Checked(object sender, RoutedEventArgs e)
        {
            themeIsDark = true;
        }

        private void radiobtnThemeLight_Initialized(object sender, EventArgs e)
        {
            radiobtnThemeLight.IsChecked = !themeIsDark;
        }

        private void radiobtnThemeLight_Checked(object sender, RoutedEventArgs e)
        {
            themeIsDark = false;
        }

        private void breaksIntervalHours_Initialized(object sender, EventArgs e)
        {
            breakIntervalHoursTemp = breaksIntervalSec / 60 / 60;
            breaksIntervalHours.Text = Convert.ToString(breakIntervalHoursTemp);
        }

        private void breaksIntervalHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakIntervalHoursTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void breaksIntervalHours_LostFocus(object sender, RoutedEventArgs e)
        {
            breaksIntervalSec = breakIntervalHoursTemp * 60 * 60 + breakIntervalMinutesTemp * 60;
        }

        private void breaksIntervalMinutes_Initialized(object sender, EventArgs e)
        {
            breakIntervalMinutesTemp = breaksIntervalSec / 60 - breakIntervalHoursTemp * 60;
            breaksIntervalMinutes.Text = Convert.ToString(breakIntervalMinutesTemp);
        }

        private void breaksIntervalMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakIntervalMinutesTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void breaksIntervalMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            breaksIntervalSec = breakIntervalHoursTemp * 60 * 60 + breakIntervalMinutesTemp * 60;
        }

        private void durationBreakHours_Initialized(object sender, EventArgs e)
        {
            breakDurationHoursTemp = durationBreakSec / 60 / 60;
            durationBreakHours.Text = Convert.ToString(breakDurationHoursTemp);
        }

        private void durationBreakHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakDurationHoursTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void durationBreakHours_LostFocus(object sender, RoutedEventArgs e)
        {
            durationBreakSec = breakDurationHoursTemp * 60 * 60 + breakDurationMinutesTemp * 60;
        }

        private void durationBreakMinutes_Initialized(object sender, EventArgs e)
        {
            breakDurationMinutesTemp = durationBreakSec / 60 - breakDurationHoursTemp * 60;
            durationBreakMinutes.Text = Convert.ToString(breakDurationMinutesTemp);
        }

        private void durationBreakMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            breakDurationMinutesTemp = Convert.ToInt32(getValueTextBox(sender));
        }

        private void durationBreakMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            durationBreakSec = breakDurationHoursTemp * 60 * 60 + breakDurationMinutesTemp * 60;
        }

        private void StartupWithSystemCheckBox_Initialized(object sender, EventArgs e)
        {
            StartupWithSystemCheckBox.IsChecked = startupWithSystem;
        }

        private void StartupWithSystemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
           startupWithSystem = true;
        }

        private void StartupWithSystemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            startupWithSystem = false;
        }

        private void CloseInTrayCheckBox_Initialized(object sender, EventArgs e)
        {
            CloseInTrayCheckBox.IsChecked = closeInTray;
        }

        private void CloseInTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            closeInTray = true;
        }

        private void CloseInTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            closeInTray = false;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(closeInTray) { 
                e.Cancel = true;
                Visibility = Visibility.Hidden;
            }
        }

        private void StartInTrayCheckBox_Initialized(object sender, EventArgs e)
        {
            StartInTrayCheckBox.IsChecked = startInTray;
        }

        private void StartInTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            startInTray = true;
        }

        private void StartInTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            startInTray = false;
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
            HideProgramCheckBox.IsChecked = hideProgram;
        }

        private void HideProgramCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Program will be hard to close & problematic undo the option", "Are you sure?", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                hideProgram = true;
            } else
            {
                hideProgram = false;
                HideProgramCheckBox.IsChecked = false;
            }
        }

        private void HideProgramCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            hideProgram = false;
        }
    }
}
