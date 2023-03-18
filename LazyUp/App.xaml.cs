using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LazyUp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string closeInTray = ConfigurationManager.AppSettings["closeInTray"];
        string shutdownModeValue;
        App()
        {
            shutdownModeValue = (closeInTray ?? "true") == "true" ? "OnExplicitShutdown" : "OnMainWindowClose";
                if (Enum.TryParse<ShutdownMode>(shutdownModeValue, out ShutdownMode shutdownMode))
                {
                    Current.ShutdownMode = shutdownMode;
                }
                else
                {
                    throw new Exception("Config error. No shutdownMode value with key closeInTray");
                }
        }
    }
}
