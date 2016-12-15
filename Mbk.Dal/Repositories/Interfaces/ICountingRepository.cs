using Mbk.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories.Interfaces
{
    public interface ICountingRepository
    {
        Task InsertAsync(IEnumerable<CountingModel> models);
    }
}
