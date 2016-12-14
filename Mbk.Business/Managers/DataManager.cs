using Mbk.Business.Interfaces;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mbk.Business
{
    public class DataManager : IDataManager
    {
        private ICameraRepository _cameraRepository;
        private IHeatMapRepository _heatMapRepository;
        private ICountingRepository _countingRepository;

        public DataManager(
            ICameraRepository cameraRepository,
            IHeatMapRepository heatMapRepository,
            ICountingRepository countingRepository)
        {
            _cameraRepository = cameraRepository;
            _heatMapRepository = heatMapRepository;
            _countingRepository = countingRepository;
        }

        public async Task<int> CollectDataAsync(string location)
        {
            var cameras = await _cameraRepository.GetAsync();

            var heatMapTasks = cameras.Select(async x => await GetHeatMap(x.Id, x.IpAddress));
            var countingTask = cameras.Select(async x => await GetCountingAsync(x.Id, x.IpAddress));
            var tasks = heatMapTasks.Union(countingTask);
            await Task.WhenAll(tasks);

            return cameras.Count;
        }

        private async Task GetHeatMap(int cameraId, string ipAddress)
        {
            throw new NotImplementedException();
        }

        private async Task GetCountingAsync(int cameraId, string ipAddress)
        {
            throw new NotImplementedException();
        }
    }
}
