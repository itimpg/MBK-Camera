using Mbk.Business.Interfaces;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using static Mbk.Helper.Converter;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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

        private async Task<string> GetFile(ConfigModel config, string uri, string ipAddress, string filename)
        {
            var queryDate = DateTime.Today;
            string fileUri = string.Format(uri, ipAddress, queryDate.Year, queryDate.Month, queryDate.Day);
            string filePath = Path.Combine(
                   config.DataConfig.Location,
                   string.Format("{0}_{1}_({2}).txt", filename, DateTime.Today.ToString("yyyyMMdd"), ipAddress.Replace('.', '-')));

            using (var client = new HttpClient())
            {
                var headerVal = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}"));
                var header = new AuthenticationHeaderValue("Basic", headerVal);
                client.DefaultRequestHeaders.Authorization = header;

                var response = await client.GetAsync(fileUri);
                response.EnsureSuccessStatusCode();
                await ReadAsFileAsync(response.Content, filePath);

                return File.ReadAllText(filePath);
            }
        }

        private async Task ReadAsFileAsync(HttpContent content, string filePath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await content.CopyToAsync(fileStream).ContinueWith((copyTask) => fileStream.Close());
                }
            }
            catch
            {
                throw;
            }
        }

        private async Task GetHeatMap(ConfigModel config, int cameraId, string ipAddress)
        {
            string[] texts = (await GetFile(config, config.HeatMapUri, ipAddress, config.HeatMapBufferFileName))
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

        private async Task GetCountingAsync(ConfigModel config, int cameraId, string ipAddress)
        {
            string[] texts = (await GetFile(config, config.CountingUri, ipAddress, config.CountingBufferFileName))
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
