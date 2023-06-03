using System.Configuration;
using System;
using System.Dynamic;
using System.Xml.Linq;
using Microsoft.CSharp.RuntimeBinder;
using System.Reflection;

namespace LazyUp {
    internal class AppConfigurator
    {
        private static AppConfigurator? instance;
        
        public static AppConfigurator GetInstance()
        {
            if (instance == null)
                instance = new AppConfigurator();
            return instance;
        }

        public AppSettings config = AppSettings.GetInstance();

        internal AppConfigurator()
        {
            ParseConfig();
        }

        public void ParseConfig()
        {
            config.LockScreenHeader = ConfigurationManager.AppSettings["lockScreenHeader"] ?? "Stroll out";
            config.LockScreenParagraph = ConfigurationManager.AppSettings["lockScreenParagraph"] ?? "Regular activity makes you live longer";
            config.ThemeIsDark = Convert.ToBoolean(ConfigurationManager.AppSettings["themeIsDark"]);
            config.BreaksIntervalSec = Convert.ToInt32(ConfigurationManager.AppSettings["breaksIntervalSec"]);
            config.DurationBreakSec = Convert.ToInt32(ConfigurationManager.AppSettings["durationBreakSec"]);
            config.StrictBreaks = Convert.ToBoolean(ConfigurationManager.AppSettings["strictBreaks"]);
            config.LookHiddenRest = Convert.ToBoolean(ConfigurationManager.AppSettings["lookHiddenRest"]);
            config.StartupWithSystem = Convert.ToBoolean(ConfigurationManager.AppSettings["startupWithSystem"]);
            config.StartInTray = Convert.ToBoolean(ConfigurationManager.AppSettings["startInTray"]);
            config.CloseInTray = Convert.ToBoolean(ConfigurationManager.AppSettings["closeInTray"]);
            config.HideProgram = Convert.ToBoolean(ConfigurationManager.AppSettings["hideProgram"]);
        }

        public void UpdateConfig(dynamic data)
        {

            PropertyInfo[] properties = typeof(AppSettings).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                try
                {
                    UpdateSetting(property.Name, property.GetValue(config).ToString());
                }
                catch (RuntimeBinderException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void UpdateSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void RefreshConfig()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}