using Mbk.Business.Interfaces;
using Mbk.Model;
using System.IO;
using System.Xml.Serialization;

namespace Mbk.Business
{
    public class ConfigManager : IConfigManager
    {
        private string DefaultConfigFilePath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "config.xml");
            }
        }

        private string DefaultBufferDirectory
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Buffer");
            }
        }

        private string DefaultDatabaseFile
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "mbk_camera.db");
            }
        }

        public ConfigModel GetConfig()
        {
            var filePath = DefaultConfigFilePath;
            var directory = DefaultBufferDirectory;
            if (!File.Exists(DefaultConfigFilePath))
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                SaveConfig(new ConfigModel
                {
                    DatabaseSource = DefaultDatabaseFile,
                    Username = "admin",
                    Password = "admin12345",
                    DataConfig = new ScheduleConfig() { Location = directory, Period = Enums.ReportPeriodType.H1 },
                    ExportConfig = new ScheduleConfig() { Period = Enums.ReportPeriodType.H1 },
                });
            }

            return XmlHelper.LoadXml<ConfigModel>(File.ReadAllText(DefaultConfigFilePath));
        }

        public void SaveConfig(ConfigModel config)
        {
            File.WriteAllText(DefaultConfigFilePath, XmlHelper.ToXml(config));
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
