using System.Threading.Tasks;

namespace Mbk.Business.Interfaces
{
    public interface IDataManager
    {
        Task<int> CollectDataAsync(string location);
    }
}
