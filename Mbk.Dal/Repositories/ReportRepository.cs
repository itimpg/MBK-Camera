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
        public async Task<IList<HeatMapCountingReportHeaderModel>> GetHeatMapCoutingReportAsync(DateTime reportDate, ReportPeriodType period)
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

                    int separateValue = 1;
                    switch (period)
                    {
                        case ReportPeriodType.M15: separateValue = 1; break;
                        case ReportPeriodType.M30: separateValue = 2; break;
                        case ReportPeriodType.H1: separateValue = 4; break;
                    }
                    var cameraData = dbData
                                    .GroupBy(x => new { x.Id, x.CameraFloor, x.CameraName, x.Date })
                                    .AsEnumerable()
                                    .Select((g) => new
                                    {
                                        Id = g.Key.Id,
                                        CameraFloor = g.Key.CameraFloor,
                                        CameraName = g.Key.CameraName,
                                        Date = g.Key.Date,
                                        Report = g
                                            .OrderBy(x => ConvertToTime(x.Time))
                                            .Select((x, i) => new
                                            {
                                                Period = i / separateValue,
                                                Time = ConvertToTime(x.Time),
                                                Density = x.Density,
                                                Population = x.Population
                                            })
                                            .GroupBy(x => x.Period)
                                            .Select(g2 => new
                                            {
                                                StartTime = g2.Min(a => a.Time),
                                                EndTime = g2.Max(a => a.Time),
                                                Density = g2.Sum(a => a.Density),
                                                Population = g2.Sum(a => a.Population)
                                            })
                                    })
                                    .OrderBy(x => x.Id);

                    var query = cameraData
                                .Select((header, index) => new HeatMapCountingReportHeaderModel
                                {
                                    CameraNo = index + 1,
                                    CameraFloor = header.CameraFloor,
                                    CameraName = header.CameraName,
                                    Date = ConvertToDate(header.Date).ToString("d/MM/yyyy"),
                                    Details = header.Report.Select(detail => new HeatMapCountingReportDetailModel
                                    {
                                        Time = string.Format("{0}-{1}",
                                            detail.StartTime.Add(TimeSpan.FromMinutes(1)).ToString("hh\\:mm"),
                                            detail.EndTime.Add(TimeSpan.FromMinutes(15)).ToString("hh\\:mm")),
                                        Density = detail.Density,
                                        Population = detail.Population
                                    }).ToList()
                                });

                    return query.ToList();
                }
            });
        }
    }
}
