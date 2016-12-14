using Mbk.Enums;
using Mbk.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbk.Dal.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<IList<HeatMapCountingReportModel>> GetHeatMapCoutingReportAsync(DateTime reportDate, ReportPeriodType period);
    }
}
