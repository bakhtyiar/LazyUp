using Microsoft.CSharp.RuntimeBinder;
using System;
using System.CodeDom;
using System.Configuration;
using System.Reflection;

namespace LazyUp
{
    internal class AppConfigurator
    {
        private static AppConfigurator? instance;

        public static AppConfigurator GetInstance()
        {
            instance ??= new AppConfigurator();
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
            config.ReviveProgram = Convert.ToBoolean(ConfigurationManager.AppSettings["reviveProgram"]);
            config.LookHiddenRest = Convert.ToBoolean(ConfigurationManager.AppSettings["lookHiddenRest"]);
            config.StartupWithSystem = Convert.ToBoolean(ConfigurationManager.AppSettings["startupWithSystem"]);
            config.StartInTray = Convert.ToBoolean(ConfigurationManager.AppSettings["startInTray"]);
            config.CloseInTray = Convert.ToBoolean(ConfigurationManager.AppSettings["closeInTray"]);
            config.HideProgram = Convert.ToBoolean(ConfigurationManager.AppSettings["hideProgram"]);
        }

        public void UpdateConfig(ref AppSettings config)
        {

            PropertyInfo[] properties = config.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                try
                {
                    string? value = property.GetValue(config).ToString();
                    if (value is not null)
                    {
                        UpdateSetting(property.Name, value);
                    } else
                    {
                        throw new Exception("Couldn't update setting. Value is null");
                    }
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