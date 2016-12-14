using Mbk.Model;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories.Interfaces
{
    public interface ICountingRepository
    {
        Task InsertAsync(CountingModel model);
    }
}
