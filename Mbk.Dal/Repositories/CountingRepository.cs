using AutoMapper;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Mbk.Dal.Repositories
{
    public class CountingRepository : ICountingRepository
    {
        public async Task InsertAsync(IEnumerable<CountingModel> models)
        {
            using (var db = new MbkCameraDb())
            {
                var countings = Mapper.Map<IEnumerable<Counting>>(models);
                db.Countings.AddRange(countings);
                await db.SaveChangesAsync();
            }
        }
    }
}
