using Mbk.Business.Interfaces;
using Mbk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Mbk.Business
{
    public class ConfigManager : IConfigManager
    {
        private string DefaultServiceName
        {
            get { return "MBK Camera Service"; }
        }

        private string DefaultConfigFilePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
            }
        }

        private string DefaultBufferDirectory
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Buffer");
            }
        }

        private string DefaultDatabaseFile
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mbk_camera.db");
            }
        }

        public ConfigModel GetConfig(string configFilePath = null)
        {
            var filePath = configFilePath ?? DefaultConfigFilePath;
            var directory = DefaultBufferDirectory;
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                SaveConfig(new ConfigModel
                {
                    ServiceName = DefaultServiceName,
                    DatabaseSource = DefaultDatabaseFile,
                    Username = "admin",
                    Password = "admin12345",
                    HeatMapUri = "http://{0}/cgi-bin/get_metadata?kind=heatmap_mov_info&mode=multi&year={1}&month={2}&date={3}&hour=0&days=1",
                    HeatMapBufferFileName = "heatmap",
                    CountingUri = "http://{0}/cgi-bin/get_metadata?kind=movcnt_info&mode=multi&year={1}&month={2}&date={3}&hour=0&days=1",
                    CountingBufferFileName = "counting",
                    DataConfig = new ScheduleConfigModel() { Location = directory, Period = Enums.ReportPeriodType.H1 },
                    ExportConfig = new ScheduleConfigModel() { Location = @"C:\", Period = Enums.ReportPeriodType.H1 },
                });
            }

            return XmlHelper.LoadXml<ConfigModel>(File.ReadAllText(filePath));
        }

        public void SaveConfig(ConfigModel config, string filePath = null)
        {
            var saveFilePath = filePath ?? DefaultConfigFilePath;
            File.WriteAllText(saveFilePath, XmlHelper.ToXml(config));
        }

        public void CheckConfig(ConfigModel config)
        {
            List<string> errorMessages = new List<string>();

            if (!Directory.Exists(config.DataConfig.Location))
            {
                errorMessages.Add($"Cannot found buffer location : {config.DataConfig.Location}");
            }

            if (!Directory.Exists(config.ExportConfig.Location))
            {
                errorMessages.Add($"Cannot found export location : {config.ExportConfig.Location}");
            }

            if (errorMessages.Count > 0)
            {
                throw new Exception(string.Join(", ", errorMessages));
            }
        }
    }

    internal static class XmlHelper
    {
        public static string ToXml<T>(T obj)
            where T : class
        {
            var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, obj);
            return stringwriter.ToString();
        }

        public static T LoadXml<T>(string xmlText)
            where T : class
        {
            var stringReader = new StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(stringReader) as T;
        }
    }
}
