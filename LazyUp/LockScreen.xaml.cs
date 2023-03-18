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

namespace LazyUp
{
    /// <summary>
    /// Interaction logic for LockScreen.xaml
    /// </summary>
    public partial class LockScreen : Window
    {
        string lockScreenHeader = ConfigurationManager.AppSettings["lockScreenHeader"];
        string lockScreenParagraph = ConfigurationManager.AppSettings["lockScreenParagraph"];
        string themeIsDark = ConfigurationManager.AppSettings["themeIsDark"];
        string durationBreakSec = ConfigurationManager.AppSettings["durationBreakSec"];
        string strictBreaks = ConfigurationManager.AppSettings["strictBreaks"];

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
            if (minutes > 0)
            {
                text += Convert.ToString(seconds) + " seconds ";
            }
            if (hours > 0 || minutes > 0)
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
            Close();
        }

        private void Header_Initialized(object sender, EventArgs e)
        {
            Header.Text = lockScreenHeader;
        }

        private void Paragraph_Initialized(object sender, EventArgs e)
        {
            Paragraph.Text = lockScreenParagraph;
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
        }

        private void TimelineBar_Initialized(object sender, EventArgs e)
        {
            TimelineBar.Value = 100;
        }
    }
}
