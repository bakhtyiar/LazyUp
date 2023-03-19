using System;
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
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;

        string lockScreenHeader = ConfigurationManager.AppSettings["lockScreenHeader"] ?? "Go move";
        string lockScreenParagraph = ConfigurationManager.AppSettings["lockScreenParagraph"] ?? "Regular activity makes you healthy and happy";
        string themeIsDark = ConfigurationManager.AppSettings["themeIsDark"] ?? "true";
        string durationBreakSec = ConfigurationManager.AppSettings["durationBreakSec"] ?? "600";
        string strictBreaks = ConfigurationManager.AppSettings["strictBreaks"] ?? "true";

        private int breakSecsLast;
        private int timeIntervalSec;
        private Timer timerIntervalForChanges;
        private Timer timerToClose;

        private delegate void TimeLastOutput();
        private void SetTimerText(TextBlock textBlock, ref int secsLast)
        {
            int hours = secsLast / 60 / 60;
            int minutes = (secsLast / 60) - (60 * hours);
            int seconds = (secsLast) - (60 * minutes) - (60 * 60 * hours);
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
        private void SetTimelineBarValue(ProgressBar timeline, int value)
        {
            timeline.Dispatcher.BeginInvoke(() =>
            {
                timeline.Value = value;
            });
        }

        public LockScreen()
        {
            breakSecsLast = Convert.ToInt32(durationBreakSec);
            timeIntervalSec = 1;

            timerIntervalForChanges = new System.Timers.Timer();
            timerIntervalForChanges.Interval = timeIntervalSec * 1000;
            timerIntervalForChanges.Elapsed += OnTimedEvent;
            timerIntervalForChanges.AutoReset = true;
            timerIntervalForChanges.Enabled = true;

            timerToClose = new System.Timers.Timer();
            timerToClose.Interval = Convert.ToInt32(durationBreakSec) * 1000;
            timerToClose.Elapsed += closeLockScreenOnTimeout;
            timerToClose.AutoReset = false;
            timerToClose.Enabled = true;

            InitializeComponent();
        }

        private void closeLockScreenOnTimeout(Object source, System.Timers.ElapsedEventArgs e)
        {
            // isWindowClosable = false;
            Dispatcher.BeginInvoke(() => { this.Visibility = Visibility.Hidden; });
            Dispatcher.BeginInvoke(() => { this.Close(); });
        }

        private void Header_Initialized(object sender, EventArgs e)
        {
            Header.Text = lockScreenHeader;
            if (themeIsDark == "true")
            {
                Header.Foreground = Brushes.White;
            } else
            {
                Header.Foreground = Brushes.Black;
            }
        }

        private void Paragraph_Initialized(object sender, EventArgs e)
        {
            Paragraph.Text = lockScreenParagraph;
            if (themeIsDark == "true")
            {
                Paragraph.Foreground = Brushes.White;
            }
            else
            {
                Paragraph.Foreground = Brushes.Black;
            }
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            breakSecsLast -= timeIntervalSec;
            SetTimerText(TimeLast, ref breakSecsLast);
            int lineLastPercentage = Convert.ToInt32((Convert.ToDouble(breakSecsLast) * 100 / Convert.ToDouble(durationBreakSec)));
            SetTimelineBarValue(TimelineBar, lineLastPercentage);
        }

        private void TimeLast_Initialized(object sender, EventArgs e)
        {
            int secsLast = Convert.ToInt32(durationBreakSec);
            SetTimerText(TimeLast, ref secsLast);
            if (themeIsDark == "true")
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
            if (themeIsDark == "true")
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
            if (themeIsDark == "true")
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
