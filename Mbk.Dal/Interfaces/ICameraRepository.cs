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

        Task<IList<Camera>> GetAsync();
        Task<Camera> GetAsync(int id);
        Task InsertAsync(Camera camera);
        Task UpdateAsync(Camera camera);
        Task DeleteAsync(int id);
    }
}
