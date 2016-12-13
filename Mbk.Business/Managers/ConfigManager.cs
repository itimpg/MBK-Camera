using Mbk.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mbk.Model;
using System.Xml.Serialization;
using System.IO;

namespace Mbk.Business
{
    public class ConfigManager : IConfigManager
    {
        private string FilePath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "config.xml");
            }
        }

        private string BufferDirectory
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Buffer");
            }
        }

        public ConfigModel GetConfig()
        {
            var filePath = FilePath;
            var directory = BufferDirectory;
            if (!File.Exists(FilePath))
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                SaveConfig(new ConfigModel
                {
                    DataConfig = new ScheduleConfig() { Location = directory, Period = Enums.ReportPeriodType.H1 },
                    ExportConfig = new ScheduleConfig() { Period = Enums.ReportPeriodType.H1 },
                });
            }

            return XmlHelper.LoadXml<ConfigModel>(File.ReadAllText(FilePath));
        }

        public void SaveConfig(ConfigModel config)
        {
            File.WriteAllText(FilePath, XmlHelper.ToXml(config));
        }
    }

    internal static class XmlHelper
    {
        public static string ToXml<T>(T obj)
            where T : class
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, obj);
            return stringwriter.ToString();
        }

        public static T LoadXml<T>(string xmlText)
            where T : class
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(stringReader) as T;
        }
    }
}
