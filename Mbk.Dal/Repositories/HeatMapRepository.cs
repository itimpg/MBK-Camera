using AutoMapper;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories
{
    public class HeatMapRepository : BaseRepository, IHeatMapRepository
    {
        public HeatMapRepository(string connectionString) 
            : base(connectionString)
        {
        }

        public async Task InsertAsync(IEnumerable<HeatMapModel> models)
        {
            using (var db = new MbkCameraDb(ConnectionString))
            {
                var heatMaps = Mapper.Map<IEnumerable<HeatMap>>(models);
                foreach (var obj in heatMaps)
                {
                    if (!db.HeatMaps.Any(x => x.CameraId == obj.CameraId && x.Date == obj.Date && x.Time == obj.Time))
                    {
                        db.HeatMaps.Add(obj);
                    }
                }
                await db.SaveChangesAsync();
            }
        }
    }
}
