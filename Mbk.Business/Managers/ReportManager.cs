using Mbk.Business.Interfaces;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Enums;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.IO;
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
            var totalCamera = source.Count();
            if (totalCamera > 0)
            {
                string timePeriod = reportPeriod == ReportPeriodType.M15 ? "15 นาที" : (reportPeriod == ReportPeriodType.M30 ? "30 นาที" : "1 ชั่วโมง");

                using (ExcelPackage package = new ExcelPackage())
                {
                    var index = package.Workbook.Worksheets.Add($"รายงานต่อ {timePeriod}");

                    int[] widths = new[] { 10, 10, 25, 20, 20, 25, 25 };
                    int[] alignCenterColumns = new[] { 1, 2, 4, 5 };
                    for (int i = 1; i <= widths.Length; i++)
                    {
                        index.Column(i).Width = widths[i - 1];
                        index.Column(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        if (alignCenterColumns.Contains(i))
                        {
                            index.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }
                    index.Cells[1, 1, 2, 7].Style.Font.Bold = true;
                    index.Cells[1, 1, 2, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    index.Cells[1, 1, 2, 7].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    index.Cells[1, 1, 2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    index.Cells[1, 1].Value = "กล้องที่";
                    index.Cells[1, 1, 2, 1].Merge = true;

                    index.Cells[1, 2].Value = "ชั้นที่";
                    index.Cells[1, 2, 2, 2].Merge = true;

                    index.Cells[1, 3].Value = "ชื่อกล้อง";
                    index.Cells[1, 3, 2, 3].Merge = true;

                    index.Cells[1, 4].Value = "วันที่";
                    index.Cells[1, 4, 2, 4].Merge = true;

                    index.Cells[1, 5].Value = "เวลา";
                    index.Cells[1, 5, 2, 5].Merge = true;

                    index.Cells[1, 6].Value = "ค่าเฉลี่ย";
                    index.Cells[1, 6, 1, 7].Merge = true;

                    index.Cells[2, 6].Value = "ความหนาแน่นของคน";
                    index.Cells[2, 7].Value = "จำนวนคน";

                    int rowIndex = 3;
                    int newCamRowIndex = rowIndex;
                    for (int i = 0; i < source.Count; i++)
                    {
                        var cam = source[i];
                        for (int j = 0; j < cam.Details.Count; j++, rowIndex++)
                        {
                            var detail = cam.Details[j];

                            index.Cells[rowIndex, 1].Value = cam.CameraNo;
                            index.Cells[rowIndex, 2].Value = cam.CameraFloor;
                            index.Cells[rowIndex, 3].Value = cam.CameraName;
                            index.Cells[rowIndex, 4].Value = cam.Date;
                            index.Cells[rowIndex, 5].Value = detail.Time;
                            index.Cells[rowIndex, 6].Value = detail.Density;
                            index.Cells[rowIndex, 7].Value = detail.Population;
                        }

                        var ws = package.Workbook.Worksheets.Add(cam.CameraName);

                        ExcelChart chart = ws.Drawings.AddChart($"chart_{i}", eChartType.ColumnClustered);
                        chart.SetPosition(1, 0, 1, 0);
                        chart.SetSize(1800, 480);
                        chart.Series.Add(
                            index.Cells[newCamRowIndex, 6, rowIndex - 1, 6],
                            index.Cells[newCamRowIndex, 5, rowIndex - 1, 5]);
                        chart.Legend.Remove();

                        ws.Cells["B27:AC27"].Merge = true;
                        ws.Cells["B27:AC27"].Value = cam.CameraName;
                        ws.Cells["B27:AC27"].Style.Font.Bold = true;
                        ws.Cells["B27:AC27"].Style.Font.UnderLine = true;
                        ws.Cells["B27:AC27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        newCamRowIndex = rowIndex;
                    }

                    string filename = string.Format("CameraReport_{0}_Per_{1}.xlsx",
                        reportDate.ToString("yyyyMMdd"),
                        reportPeriod == ReportPeriodType.M15 ? "15Minutes" : (reportPeriod == ReportPeriodType.M30 ? "30Minutes" : "1Hour"));
                    string exportFullPath = Path.Combine(reportLocation, filename);
                    package.SaveAs(new FileInfo(exportFullPath));
                }
            }
            return totalCamera;
        }
    }
}
