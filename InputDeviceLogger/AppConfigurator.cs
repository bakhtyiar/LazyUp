using Microsoft.CSharp.RuntimeBinder;
using System;
using System.CodeDom;
using System.Configuration;
using System.Reflection;

namespace InputDeviceLogger
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
            config.LogsPath = ConfigurationManager.AppSettings["LogsPath"] ?? "./defaultInputLogs/";
            config.LoggingMode = ConfigurationManager.AppSettings["LoggingMode"] ?? "DontDisturb";
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