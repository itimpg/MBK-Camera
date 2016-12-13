using Mbk.Business.Interfaces;
using Mbk.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mbk.Business
{
    public class ReportManager : IReportManager
    {
        public async Task<int> GenerateDataReportAsync(string reportLocation, DateTime reportDate, ReportPeriodType reportPeriod)
        {
            return await Task.Run(() =>
               {
                   Thread.Sleep(5000);
                   return 20;
               });
        }

        public async Task GenerateHeatMapReportAsync(string location)
        {

        }
    }
}
