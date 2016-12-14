using Mbk.Business.Interfaces;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mbk.Business
{
    public class ReportManager : IReportManager
    {
        private IHeatMapRepository _heatmapRepository;
        private ICountingRepository _countingRepository;

        public ReportManager(
            IHeatMapRepository heatMapRepository,
            ICountingRepository countingRepository)
        {
            _heatmapRepository = heatMapRepository;
            _countingRepository = countingRepository;
        }

        public async Task<int> GenerateDataReportAsync(
            string reportLocation,
            DateTime reportDate,
            ReportPeriodType reportPeriod)
        {
            return await Task.Run(() =>
               {
                   Thread.Sleep(5000);
                   return 20;
               });
        }
    }
}
