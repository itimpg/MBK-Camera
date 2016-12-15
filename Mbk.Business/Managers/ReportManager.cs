using Mbk.Business.Interfaces;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Enums;
using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Mbk.Business
{
    public class ReportManager : IReportManager
    {
        private IReportRepository _reportRepository;

        public ReportManager(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<int> GenerateDataReportAsync(
            string reportLocation,
            DateTime reportDate,
            ReportPeriodType reportPeriod)
        {
            var source = await _reportRepository.GetHeatMapCoutingReportAsync(reportDate, reportPeriod);
            
            // TODO: create Excel file 

            var totalCamera = source.Count();
            return totalCamera;
        }
    }
}
