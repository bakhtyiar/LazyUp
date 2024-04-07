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

namespace InputDeviceLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly private AppConfigurator _appConfigurator = AppConfigurator.GetInstance();
        private AppSettings _appSettings = AppSettings.GetInstance();

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

        private void PathToLogs_Initialized(object sender, EventArgs e)
        {
            PathToLogs.Text = _appSettings.LogsPath;
        }

        private void PathToLogs_TextChanged(object sender, TextChangedEventArgs e)
        {
            _appSettings.LogsPath = getValueTextBox(sender);
        }

        private void DontDisturbRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void DisturbRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void StartRecordButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AnalyzeLogsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
