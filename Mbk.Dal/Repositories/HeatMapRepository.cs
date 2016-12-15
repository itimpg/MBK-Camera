using AutoMapper;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories
{
    public class HeatMapRepository : IHeatMapRepository
    {
        public async Task InsertAsync(IEnumerable<HeatMapModel> models)
        {
            using (var db = new MbkCameraDb())
            {
                var heatMaps = Mapper.Map<IEnumerable<HeatMap>>(models);
                db.HeatMaps.AddRange(heatMaps);
                await db.SaveChangesAsync();
            }
        }
    }
}
