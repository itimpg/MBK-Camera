using Mbk.Enums;
using Mbk.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<IList<HeatMapCountingReportHeaderModel>> GetHeatMapCoutingReportAsync(DateTime reportDate, ReportPeriodType period);
    }
}
