using Mbk.Dal.Repositories.Interfaces;
using Mbk.Enums;
using Mbk.Helper;
using Mbk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Mbk.Helper.Converter;

namespace Mbk.Dal
{
    public class ReportRepository : IReportRepository
    {
        public async Task<IList<HeatMapCountingReportModel>> GetHeatMapCoutingReportAsync(DateTime reportDate, ReportPeriodType period)
        {
            return await Task.Run(() =>
            {
                using (var db = new MbkCameraDb())
                {
                    string queryDate = ToDateString(reportDate);
                    var dbData = from cam in db.Cameras
                                 join hm in db.HeatMaps on cam.Id equals hm.CameraId
                                 join ct in db.Countings on new { CameraId = cam.Id, Date = hm.Date, Time = hm.Time }
                                    equals new { CameraId = ct.CameraId, Date = ct.Date, ct.Time }
                                 where ct.Date == queryDate
                                 select new
                                 {
                                     Id = cam.Id,
                                     CameraFloor = cam.Floor,
                                     CameraName = cam.Name,
                                     Date = ct.Date,
                                     Time = ct.Time,
                                     Density = hm.Density,
                                     Population = ct.Population
                                 };

                    var checker = dbData.ToArray();

                    int modValue = 1;
                    switch (period)
                    {
                        case ReportPeriodType.M15: modValue = 1; break;
                        case ReportPeriodType.M30: modValue = 2; break;
                        case ReportPeriodType.H1: modValue = 4; break;
                    }
                    var rawData = dbData.AsEnumerable()
                                .Select((x, i) => new
                                {
                                    RowNum = i % modValue,
                                    Id = x.Id,
                                    CameraFloor = x.CameraFloor,
                                    CameraName = x.CameraName,
                                    Date = x.Date,
                                    Time = ConvertToTime(x.Time),
                                    Density = x.Density,
                                    Population = x.Population
                                }).ToArray();

                    var query = rawData
                                .GroupBy(x => new { x.RowNum, x.Id, x.CameraFloor, x.CameraName, x.Date })
                                .Select(g => new
                                {
                                    Id = g.Key.Id,
                                    CameraFloor = g.Key.CameraFloor,
                                    CameraName = g.Key.CameraName,
                                    Date = g.Key.Date,
                                    StartTime = g.Min(x => x.Time),
                                    EndTime = g.Max(x => x.Time),
                                    Density = g.Sum(x => x.Density),
                                    Population = g.Sum(x => x.Population)
                                })
                                .OrderBy(x => x.Id).ThenBy(x => x.StartTime)
                                .Select((x, index) => new HeatMapCountingReportModel
                                {
                                    CameraFloor = x.CameraFloor,
                                    CameraName = x.CameraName,
                                    Date = ConvertToDate(x.Date).ToString("d/MM/yyyy"),
                                    Time = string.Format("{0}-{1}",
                                        x.StartTime.Add(TimeSpan.FromMinutes(1)).ToString("hh\\:mm"),
                                        x.EndTime.ToString("hh\\:mm")),
                                    Density = x.Density,
                                    Population = x.Population
                                });
                    
                    return query.ToList();
                }
            });
        }
    }
}
