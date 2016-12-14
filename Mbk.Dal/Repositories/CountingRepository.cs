using AutoMapper;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories
{
    public class CountingRepository : ICountingRepository
    {
        public async Task<List<CountingModel>> GetAsync(int cameraId)
        {
            return await Task.Run(() =>
            {
                using (var db = new MbkCameraDb())
                {
                    var countings = db.Countings.Where(x => x.CameraId == cameraId).ToList();
                    return Mapper.Map<List<CountingModel>>(countings);
                }
            });
        }

        public async Task InsertAsync(IList<CountingModel> models)
        {
            using (var db = new MbkCameraDb())
            {
                var countings = Mapper.Map<IList<Counting>>(models);
                db.Countings.AddRange(countings);
                await db.SaveChangesAsync();
            }
        }
    }
}
