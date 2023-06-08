﻿using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;
using System.Timers;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using System.Windows.Interop;

namespace LazyUp
{
    /// <summary>
    /// Interaction logic for LockScreen.xaml
    /// </summary>
    public partial class LockScreen : Window
    {
        //Hack code for removing opportunity to close opened LockScreen.xaml window
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;

        readonly private AppSettings _config = AppSettings.GetInstance();

        private int _breakSecsLast;
        readonly private int _timeIntervalSec;
        readonly private Timer _timerIntervalForChanges;
        readonly private Timer _timerToClose;

        private delegate void TimeLastOutput();
        static private void SetTimerText(TextBlock textBlock, ref int secsLast)
        {
            TimeSpan ts = TimeSpan.FromSeconds(secsLast);
            int hours = ts.Hours;
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;
            string text = "";
            if (hours > 0)
            {
                text += Convert.ToString(hours) + " hours ";
            }
            if (minutes > 0)
            {
                text += Convert.ToString(minutes) + " minutes ";
            }
            if (seconds > 0)
            {
                text += Convert.ToString(seconds) + " seconds ";
            }
            if (hours > 0 || minutes > 0 || seconds > 0)
            {
                text += "left";
            }
            textBlock.Dispatcher.BeginInvoke(() => {
                textBlock.Text = text;
            });
        }
        
        private delegate void TimelineOutput();
        static private void SetTimelineBarValue(ProgressBar timeline, int value)
        {
            timeline.Dispatcher.BeginInvoke(() =>
            {
                timeline.Value = value;
            });
        }

        public LockScreen()
        {
            _breakSecsLast = _config.DurationBreakSec;
            _timeIntervalSec = 1;

            _timerIntervalForChanges = new System.Timers.Timer();
            _timerIntervalForChanges.Interval = _timeIntervalSec * 1000;
            _timerIntervalForChanges.Elapsed += OnTimedEvent;
            _timerIntervalForChanges.AutoReset = true;
            _timerIntervalForChanges.Enabled = true;

            _timerToClose = new System.Timers.Timer();
            _timerToClose.Interval = _config.DurationBreakSec * 1000;
            _timerToClose.Elapsed += CloseLockScreenOnTimeout;
            _timerToClose.AutoReset = false;
            _timerToClose.Enabled = true;

            InitializeComponent();
        }

        private void CloseLockScreenOnTimeout(Object? source, System.Timers.ElapsedEventArgs e)
        {
            // isWindowClosable = false;
            Dispatcher.BeginInvoke(() => { this.Visibility = Visibility.Hidden; });
            Dispatcher.BeginInvoke(() => { this.Close(); });
        }

        private void Header_Initialized(object sender, EventArgs e)
        {
            Header.Text = _config.LockScreenHeader;
            if (_config.ThemeIsDark)
            {
                Header.Foreground = Brushes.White;
            } else
            {
                Header.Foreground = Brushes.Black;
            }
        }

        private void Paragraph_Initialized(object sender, EventArgs e)
        {
            Paragraph.Text = _config.LockScreenParagraph;
            if (_config.ThemeIsDark)
            {
                Paragraph.Foreground = Brushes.White;
            }
            else
            {
                Paragraph.Foreground = Brushes.Black;
            }
        }

        private void OnTimedEvent(Object? source, System.Timers.ElapsedEventArgs e)
        {
            _breakSecsLast -= _timeIntervalSec;
            SetTimerText(TimeLast, ref _breakSecsLast);
            _config.DurationBreakSec = _config.DurationBreakSec == 0 ? 1 : _config.DurationBreakSec;
            int lineLastPercentage = _breakSecsLast * 100 / (_config.DurationBreakSec);
            SetTimelineBarValue(TimelineBar, lineLastPercentage);
        }

        private void TimeLast_Initialized(object sender, EventArgs e)
        {
            int secsLast = _config.DurationBreakSec;
            SetTimerText(TimeLast, ref secsLast);
            if (_config.ThemeIsDark)
            {
                TimeLast.Foreground = Brushes.White;
            }
            else
            {
                TimeLast.Foreground = Brushes.Black;
            }
        }

        private void TimelineBar_Initialized(object sender, EventArgs e)
        {
            TimelineBar.Value = 100;
            if (_config.ThemeIsDark)
            {
                TimelineBar.Foreground = Brushes.White;
            }
            else
            {
                TimelineBar.Foreground = Brushes.Black;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
            var helper = new WindowInteropHelper(this).Handle;
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
            window.Activate();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (_config.ThemeIsDark)
            {
                this.Background = Brushes.Black;
            }
            else
            {
                this.Background = Brushes.White;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Topmost = true;
            this.Top = 0;
            this.Left = 0;
            this.ShowInTaskbar = false;
        }
    }
}
