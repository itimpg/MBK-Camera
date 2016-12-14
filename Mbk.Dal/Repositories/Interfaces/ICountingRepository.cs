using Mbk.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories.Interfaces
{
    public interface ICountingRepository
    {
        Task<List<CountingModel>> GetAsync(int cameraId);
        Task InsertAsync(IList<CountingModel> models);
    }
}
