using Mbk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbk.Dal.Interfaces
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
