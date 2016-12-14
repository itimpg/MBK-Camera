using AutoMapper;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories
{
    public class CountingRepository : ICountingRepository
    {
        public async Task InsertAsync(CountingModel model)
        {
            using (var db = new MbkCameraDb())
            {
                var counting = Mapper.Map<Counting>(model);
                db.Countings.Add(counting);
                await db.SaveChangesAsync();
            }
        }
    }
}
