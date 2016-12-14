using Mbk.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories.Interfaces
{
    public interface ICameraRepository
    {
        bool CheckConnection();

        Task<IList<CameraModel>> GetAsync();
        Task<CameraModel> GetAsync(int id);
        Task InsertAsync(CameraModel camera);
        Task UpdateAsync(CameraModel camera);
        Task DeleteAsync(int id);
    }
}
