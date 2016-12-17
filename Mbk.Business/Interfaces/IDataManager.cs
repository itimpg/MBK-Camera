using Mbk.Model;
using System.Threading.Tasks;

namespace Mbk.Business.Interfaces
{
    public interface IDataManager
    {
        Task CollectDataAsync(string location, CameraModel camera);
    }
}
