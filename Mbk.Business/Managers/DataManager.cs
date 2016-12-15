using Mbk.Business.Interfaces;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Globalization;
using System.IO;

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

            return cameras.Count();
        }

        private async Task GetHeatMap(int cameraId, string ipAddress)
        {
            string[] texts = (await GetHeatMapFile(ipAddress))
                .Split(new[] { "--myboundary" }, StringSplitOptions.RemoveEmptyEntries);

            var periods =
                texts.Select(x => x
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Length > 1)
                    .Select(x => new
                    {
                        Gmt = x[3].Split(',')[5],
                        DateTime = DateTime.ParseExact(
                            string.Join(" ", x[3].Split(',').Take(2)),
                            "yyyyMMdd HHmm", CultureInfo.InvariantCulture),
                        Raw = string.Join(Environment.NewLine, x.Skip(4)),
                        Value = string.Join(Environment.NewLine, x.Skip(4))
                            .Split(new[] { Environment.NewLine, "," }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(a =>
                            {
                                decimal value = 0;
                                decimal.TryParse(a, out value);
                                return value;
                            })
                            .Where(a => a > 0).ToArray()
                    }).Select(x => new
                    {
                        Gmt = x.Gmt.StartsWith("-") ?
                            TimeSpan.Parse(x.Gmt.Remove(0, 1)).Negate() :
                            TimeSpan.Parse(x.Gmt.Remove(0, 1)),
                        DateTime = x.DateTime,
                        RawData = x.Raw,
                        Density = x.Value.Length > 0 ? x.Value.Sum() / x.Value.Length : 0,
                    });

            var heatMap = new HeatMapModel
            {
                CameraId = cameraId,
            };
            await _heatMapRepository.InsertAsync(heatMap);
        }

        private async Task<string> GetHeatMapFile(string ipAddress)
        {
            // TODO: Get data by command 
            // http://admin:admin12345@192.168.13.32/cgi-bin/get_metadata?kind=heatmap_mov&mode=multi&year=2016&month=12&date=5&hour=0&days=1
            return await Task.Run(() =>
            {
                return File.ReadAllText(@"D:\mbk\heatmap.txt");
            });
        }
        private async Task<string> GetCountingFile(string ipAddress)
        {
            // TODO: Get data by command
            // http://admin:admin12345@192.168.13.32/cgi-bin/get_metadata?kind=movcnt_info&mode=multi&year=2016&month=12&date=8&hour=0&days=1
            return await Task.Run(() =>
            {
                return File.ReadAllText(@"D:\mbk\counting.txt");
            });
        }

        private async Task GetCountingAsync(int cameraId, string ipAddress)
        {
            string[] texts = (await GetHeatMapFile(ipAddress))
               .Split(new[] { "--myboundary" }, StringSplitOptions.RemoveEmptyEntries);

            var periods =
                texts.Select(x => x
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Length > 1)
                    .Select(x => new
                    {
                        Gmt = x[3].Split(',')[5],
                        DateTime = DateTime.ParseExact(
                            string.Join(" ", x[3].Split(',').Take(2)),
                            "yyyyMMdd HHmm", CultureInfo.InvariantCulture),
                        Raw = string.Join(Environment.NewLine, x.Skip(4)),
                        Value = x.Skip(4)
                            .Select(a =>
                            {
                                var items = a.Split(',').Select(decimal.Parse).ToArray();
                                return items[4] - items[5];
                            })
                            .Sum()
                    })
                    .Select(x => new
                    {
                        Gmt = x.Gmt.StartsWith("-") ?
                            TimeSpan.Parse(x.Gmt.Remove(0, 1)).Negate() :
                            TimeSpan.Parse(x.Gmt.Remove(0, 1)),
                        DateTime = x.DateTime,
                        RawData = x.Raw,
                        Population = x.Value,
                    });

            var counting = new CountingModel
            {
                CameraId = cameraId,
            };
            await _countingRepository.InsertAsync(counting);
        }
    }
}
