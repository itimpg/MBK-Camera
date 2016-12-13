using Mbk.Model;

namespace Mbk.Business.Interfaces
{
    public interface IConfigManager
    {
        Config GetConfig();
        void SaveConfig(Config config);
    }
}
