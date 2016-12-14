using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbk.Model
{
    public class HeatMapCountingReportModel
    {
        public int CameraNo { get; set; }
        public string CameraFloor { get; set; }
        public string CameraName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public decimal Density { get; set; }
        public decimal Population { get; set; }
    }
}
