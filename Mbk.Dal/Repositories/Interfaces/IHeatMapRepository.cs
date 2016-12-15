using Mbk.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories.Interfaces
{
    public interface IHeatMapRepository
    {
        Task InsertAsync(IEnumerable<HeatMapModel> models);
    }
}
