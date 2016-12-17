using Mbk.Model;

namespace Mbk.Business.Interfaces
{
    public interface IConfigManager
    {
        ConfigModel GetConfig(string configFilePath = null);
        void SaveConfig(ConfigModel config, string filePath = null);
    }
}
