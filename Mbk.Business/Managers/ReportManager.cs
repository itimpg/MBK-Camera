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

        private ExcelWorksheet InitHeatmap(ExcelPackage package)
        {
            var heatmapSheet = package.Workbook.Worksheets.Add("รายงานความหนาแน่นของคน");

            int[] widths = new[] { 10, 10, 25, 20, 20, 25, 20, 38 };
            int[] alignCenterColumns = new[] { 1, 2, 4, 5 };
            for (int i = 1; i <= widths.Length; i++)
            {
                heatmapSheet.Column(i).Width = widths[i - 1];
                heatmapSheet.Column(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                if (alignCenterColumns.Contains(i))
                {
                    heatmapSheet.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }
            var headers = heatmapSheet.Cells[1, 1, 2, 8];
            headers.Style.Font.Bold = true;
            headers.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headers.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            headers.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headers.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            string[] columns = new[] { "กล้องที่", "ชั้นที่", "ชื่อกล้อง", "วันที่", "เวลา" };
            for (int i = 0; i < columns.Length; i++)
            {
                int col = i + 1;
                heatmapSheet.Cells[1, col].Value = columns[i];
                heatmapSheet.Cells[1, col, 2, col].Merge = true;
            }

            heatmapSheet.Cells[1, 6].Value = "ค่าเฉลี่ย";
            heatmapSheet.Cells[2, 6].Value = "ความหนาแน่นของคน";

            heatmapSheet.Cells[1, 7].Value = "พื้นที่";
            heatmapSheet.Cells[2, 7].Value = "(ตารางมตร)";

            heatmapSheet.Cells[1, 8].Value = "ค่าเฉลี่ย";
            heatmapSheet.Cells[2, 8].Value = "ความหนาแน่นของคน/พื้นที่(ตารางมตร)";

            return heatmapSheet;
        }
        private ExcelWorksheet InitCounting(ExcelPackage package)
        {
            var countingSheet = package.Workbook.Worksheets.Add("รายงานจำนวนคน");

            int[] widths = new[] { 10, 10, 25, 20, 20 };
            int[] alignCenterColumns = new[] { 1, 2, 4, 5 };
            for (int i = 1; i <= widths.Length; i++)
            {
                countingSheet.Column(i).Width = widths[i - 1];
                countingSheet.Column(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                if (alignCenterColumns.Contains(i))
                {
                    countingSheet.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }
            var headers = countingSheet.Cells[1, 1, 2, 31];
            headers.Style.Font.Bold = true;
            headers.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headers.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            headers.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headers.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            string[] columns = new[] { "กล้องที่", "ชั้นที่", "ชื่อกล้อง", "วันที่", "เวลา" };
            for (int i = 0; i < columns.Length; i++)
            {
                int col = i + 1;
                countingSheet.Cells[1, col].Value = columns[i];
                countingSheet.Cells[1, col, 2, col].Merge = true;
            }

            for (int i = 0; i < 12; i++)
            {
                int column = (i * 2) + 6;

                countingSheet.Cells[1, column].Value = $"Line{i + 1}";
                countingSheet.Cells[1, column, 1, column + 1].Merge = true;
                countingSheet.Cells[2, column].Value = "A -> B";
                countingSheet.Cells[2, column + 1].Value = "B -> A";
            }

            return countingSheet;
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
                    var heatmapSheet = InitHeatmap(package);
                    var countingSheet = InitCounting(package);

                    int rowIndex = 3;
                    int newCamRowIndex = rowIndex;

                    // loop for print data to excel
                    for (int i = 0; i < source.Count; i++)
                    {
                        var cam = source[i];
                        for (int j = 0; j < cam.Details.Count; j++, rowIndex++)
                        {
                            var detail = cam.Details[j];

                            // print heatmap
                            heatmapSheet.Cells[rowIndex, 1].Value = cam.CameraNo;
                            heatmapSheet.Cells[rowIndex, 2].Value = cam.CameraFloor;
                            heatmapSheet.Cells[rowIndex, 3].Value = cam.CameraName;
                            heatmapSheet.Cells[rowIndex, 4].Value = cam.Date;
                            heatmapSheet.Cells[rowIndex, 5].Value = detail.Time;
                            heatmapSheet.Cells[rowIndex, 6].Value = detail.Density;
                            heatmapSheet.Cells[rowIndex, 7].Value = detail.Area;
                            heatmapSheet.Cells[rowIndex, 8].Value = detail.DensityPerArea;

                            // print counting
                            countingSheet.Cells[rowIndex, 1].Value = cam.CameraNo;
                            countingSheet.Cells[rowIndex, 2].Value = cam.CameraFloor;
                            countingSheet.Cells[rowIndex, 3].Value = cam.CameraName;
                            countingSheet.Cells[rowIndex, 4].Value = cam.Date;
                            countingSheet.Cells[rowIndex, 5].Value = detail.Time;
                            for (int k = 0; k < detail.Countings.Count; k++)
                            {
                                var counting = detail.Countings[k];
                                int column = (k * 2) + 6;
                                countingSheet.Cells[rowIndex, column].Value = counting.A;
                                countingSheet.Cells[rowIndex, column + 1].Value = counting.B;
                            }
                        }

                        // create chart by heatmap data
                        var ws = package.Workbook.Worksheets.Add($"กล้องที่{cam.CameraNo} {cam.CameraName}");
                        ExcelChart chart = ws.Drawings.AddChart($"chart_{i}", eChartType.ColumnClustered);
                        chart.SetPosition(1, 0, 1, 0);
                        chart.SetSize(1800, 480);
                        chart.Series.Add(
                            heatmapSheet.Cells[newCamRowIndex, 6, rowIndex - 1, 6],
                            heatmapSheet.Cells[newCamRowIndex, 5, rowIndex - 1, 5]);
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
