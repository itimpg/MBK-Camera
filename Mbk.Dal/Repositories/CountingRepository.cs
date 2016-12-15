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
        public async Task InsertAsync(IEnumerable<CountingModel> models)
        {
            using (var db = new MbkCameraDb())
            {
                var countings = Mapper.Map<IEnumerable<Counting>>(models);
                foreach (var obj in countings)
                {
                    if (!db.Countings.Any(x => x.CameraId == obj.CameraId && x.Date == obj.Date && x.Time == obj.Time))
                    {
                        db.Countings.Add(obj);
                    }
                }
                await db.SaveChangesAsync();
            }
        }
    }
}
