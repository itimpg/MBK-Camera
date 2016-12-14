using AutoMapper;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories
{
    public class HeatMapRepository : IHeatMapRepository
    {
        public async Task InsertAsync(HeatMapModel model)
        {
            using (var db = new MbkCameraDb())
            {
                var heatMap = Mapper.Map<HeatMap>(model);
                db.HeatMaps.Add(heatMap);
                await db.SaveChangesAsync();
            }
        }
    }
}
