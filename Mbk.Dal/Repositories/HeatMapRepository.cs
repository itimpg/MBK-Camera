using AutoMapper;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories
{
    public class HeatMapRepository : IHeatMapRepository
    {
        public async Task<List<HeatMapModel>> GetAsync(int cameraId)
        {
            return await Task.Run(() =>
            {
                using (var db = new MbkCameraDb())
                {
                    var heatMaps = db.HeatMaps.Where(x => x.CameraId == cameraId).ToList();
                    return Mapper.Map<List<HeatMapModel>>(heatMaps);
                }
            });
        }

        public async Task InsertAsync(IList<HeatMapModel> models)
        {
            using (var db = new MbkCameraDb())
            {
                var heatMaps = Mapper.Map<IList<HeatMap>>(models);
                db.HeatMaps.AddRange(heatMaps);
                await db.SaveChangesAsync();
            }
        }
    }
}
