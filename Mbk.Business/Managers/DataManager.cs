using Mbk.Business.Interfaces;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Mbk.Helper.Converter;

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
            await GetHeatMap(config, camera);
            await GetCountingAsync(config, camera);
        }

        private async Task<string> GetFile(ConfigModel config, CameraModel camera, string uri, string filename)
        {
            var queryDate = DateTime.Now.AddHours(-7).Date;
            string fileUri = string.Format(uri, camera.IpAddress, queryDate.Year, queryDate.Month, queryDate.Day);
            string filePath = Path.Combine(
                   config.DataConfig.Location,
                   string.Format("{0}_{1}_({2}).txt", filename, queryDate.ToString("yyyyMMdd"), camera.IpAddress.Replace('.', '-')));

            using (var client = new HttpClient())
            {
                var headerVal = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{camera.Username}:{camera.Password}"));
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

        private async Task GetHeatMap(ConfigModel config, CameraModel camera)
        {
            string[] texts = (await GetFile(config, camera, config.HeatMapUri, config.HeatMapBufferFileName))
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
                            .Select(long.Parse)
                            .Where(a => a > 0).ToArray()
                    }).Select(x => new HeatMapModel
                    {
                        CameraId = camera.Id,
                        Gmt = x.Gmt,
                        DateTime = x.DateTime,
                        //RawData = x.Raw,
                        RawData = "Leave it empty",
                        TotalValue = x.Value.Sum(),
                        TotalCount = x.Value.Length,
                        Density = x.Value.Length > 0 ?
                            Math.Round((decimal)(x.Value.Sum() / x.Value.Length)) : 0,
                    }).ToArray();
            await _heatMapRepository.InsertAsync(heatmaps);
        }

        private async Task GetCountingAsync(ConfigModel config, CameraModel camera)
        {
            string[] texts = (await GetFile(config, camera, config.CountingUri, config.CountingBufferFileName))
               .Split(new[] { "--myboundary" }, StringSplitOptions.RemoveEmptyEntries);

            var countings =
                texts.Select(x => x.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Length > 1)
                    .Select(x => new CountingModel
                    {
                        CameraId = camera.Id,
                        Gmt = ConvertToTime(x[3].Split(',')[5]),
                        DateTime = ConvertToDateTime(string.Join(" ", x[3].Split(',').Take(2))),
                        //RawData = string.Join(Environment.NewLine, x.Skip(4)),
                        RawData = "Leave it empty",
                        CountingDetails = x.Skip(4).Select(a =>
                        {
                            var items = a.Split(',').Select(long.Parse).ToArray();
                            return new CountingDetailModel { A = items[4], B = items[5] };
                        }).ToList()
                    }).ToArray();
            await _countingRepository.InsertAsync(countings);
        }
    }
}
