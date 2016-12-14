using Mbk.Business.Interfaces;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
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
            // TODO: Get data by command 
            // http://admin:admin12345@192.168.13.32/cgi-bin/get_metadata?kind=heatmap_mov&mode=multi&year=2016&month=12&date=5&hour=0&days=1

            // TODO: fill data to save
            var heatMap = new HeatMapModel
            {
                CameraId = cameraId,
            };
            await _heatMapRepository.InsertAsync(heatMap);
        }

        private async Task GetCountingAsync(int cameraId, string ipAddress)
        {
            // TODO: Get data by command
            // http://admin:admin12345@192.168.13.32/cgi-bin/get_metadata?kind=movcnt_info&mode=multi&year=2016&month=12&date=8&hour=0&days=1

            // TODO: fill data to save
            var counting = new CountingModel
            {
                CameraId = cameraId,
            };
            await _countingRepository.InsertAsync(counting);
        }
    }
}
