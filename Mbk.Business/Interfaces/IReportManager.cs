﻿using Mbk.Enums;
using System;
using System.Threading.Tasks;

namespace Mbk.Business.Interfaces
{
    public interface IReportManager
    {
        Task GenerateHeatMapReportAsync(string location);
        Task<int> GenerateDataReportAsync(string reportLocation, DateTime reportDate, ReportPeriodType reportPeriod);
    }
}
