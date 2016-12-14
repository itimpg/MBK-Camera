using Mbk.Model;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories.Interfaces
{
    public interface IHeatMapRepository
    {
        Task InsertAsync(HeatMapModel model);
    }
}
