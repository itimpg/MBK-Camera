using Mbk.Business.Interfaces;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using static Mbk.Helper.Converter;
using System.Net.Http;

namespace Mbk.Business
{
    public class DataManager : IDataManager
    {
        private IHeatMapRepository _heatMapRepository;
        private ICountingRepository _countingRepository;

        public DataManager(
            IHeatMapRepository heatMapRepository,
            ICountingRepository countingRepository)
        {
            _heatMapRepository = heatMapRepository;
            _countingRepository = countingRepository;
        }

        public async Task CollectDataAsync(ConfigModel config, CameraModel camera)
        {
            await GetHeatMap(config, camera.Id, camera.IpAddress);
            await GetCountingAsync(config, camera.Id, camera.IpAddress);
        }

        private async Task<string> GetHeatMapFile(ConfigModel config, string ipAddress)
        {
            return await Task.Run(() =>
            {
                return File.ReadAllText(@"D:\mbk\heatmap.txt");
            });

            // TODO: Get data by command
            using (var client = new HttpClient())
            {
                string uri = $@"http://{config.Username}:{config.Password}@{ipAddress}/cgi-bin/get_metadata?kind=heatmap_mov&mode=multi&year=2016&month=12&date=8&hour=0&days=1";
                using (var stream = await client.GetStreamAsync(uri))
                {
                    string filePath = Path.Combine(config.DataConfig.Location, $"heatmap_{DateTime.Today.ToString("yyyyMMdd")}.txt");
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                        return File.ReadAllText(filePath);
                    }
                }
            }
        }
        private async Task GetHeatMap(ConfigModel config, int cameraId, string ipAddress)
        {
            string[] texts = (await GetHeatMapFile(config, ipAddress))
                .Split(new[] { "--myboundary" }, StringSplitOptions.RemoveEmptyEntries);

            var heatmaps =
                texts.Select(x => x
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Length > 1)
                    .Select(x => new
                    {
                        Gmt = ConvertToTime(x[3].Split(',')[5]),
                        DateTime = ConvertToDateTime(string.Join(" ", x[3].Split(',').Take(2))),
                        Raw = string.Join(Environment.NewLine, x.Skip(4)),
                        Value = string.Join(Environment.NewLine, x.Skip(4))
                            .Split(new[] { Environment.NewLine, "," }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(decimal.Parse)
                            .Where(a => a > 0).ToArray()
                    }).Select(x => new HeatMapModel
                    {
                        CameraId = cameraId,
                        Gmt = x.Gmt,
                        DateTime = x.DateTime,
                        //RawData = x.Raw,
                        RawData = "Leave it empty",
                        Density = x.Value.Length > 0 ? x.Value.Sum() / x.Value.Length : 0,
                    }).ToArray();
            await _heatMapRepository.InsertAsync(heatmaps);
        }

        private async Task<string> GetCountingFile(ConfigModel config, string ipAddress)
        {
            return await Task.Run(() =>
            {
                return File.ReadAllText(@"D:\mbk\counting.txt");
            });

            // TODO: Get data by command
            using (var client = new HttpClient())
            {
                string uri = $@"http://{config.Username}:{config.Password}@{ipAddress}/cgi-bin/get_metadata?kind=movcnt_info&mode=multi&year=2016&month=12&date=8&hour=0&days=1";
                using (var stream = await client.GetStreamAsync(uri))
                {
                    string filePath = Path.Combine(config.DataConfig.Location, $"counting_{DateTime.Today.ToString("yyyyMMdd")}.txt");
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                        return File.ReadAllText(filePath);
                    }
                }
            }
        }
        private async Task GetCountingAsync(ConfigModel config, int cameraId, string ipAddress)
        {
            string[] texts = (await GetHeatMapFile(config, ipAddress))
               .Split(new[] { "--myboundary" }, StringSplitOptions.RemoveEmptyEntries);

            var countings =
                texts.Select(x => x.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Length > 1)
                    .Select(x => new CountingModel
                    {
                        CameraId = cameraId,
                        Gmt = ConvertToTime(x[3].Split(',')[5]),
                        DateTime = ConvertToDateTime(string.Join(" ", x[3].Split(',').Take(2))),
                        //RawData = string.Join(Environment.NewLine, x.Skip(4)),
                        RawData = "Leave it empty",
                        Population = x.Skip(4).Sum(a =>
                        {
                            var items = a.Split(',').Select(decimal.Parse).ToArray();
                            return items[4] - items[5];
                        })
                    }).ToArray();
            await _countingRepository.InsertAsync(countings);
        }
    }
}
