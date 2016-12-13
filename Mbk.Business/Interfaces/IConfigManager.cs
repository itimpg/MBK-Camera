using Mbk.Model;

namespace Mbk.Business.Interfaces
{
    public interface IConfigManager
    {
        ConfigModel GetConfig();
        void SaveConfig(ConfigModel config);
    }
}
