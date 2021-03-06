﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbk.Model
{
    public class HeatMapCountingReportHeaderModel
    {
        public int CameraNo { get; set; }
        public string CameraFloor { get; set; }
        public string CameraName { get; set; }
        public string Date { get; set; }

        public IList<HeatMapCountingReportDetailModel> Details { get; set; }
    }

    public class HeatMapCountingReportDetailModel
    {
        public string Time { get; set; }
        public decimal Density { get; set; }
        public decimal DensityPerArea { get; set; }
        public decimal Area { get; set; }
        public IList<CountingReportDetailModel> Countings { get; set; }
    }

    public class CountingReportDetailModel
    {
        public int LineNo { get; set; }
        public long A { get; set; }
        public long B { get; set; }
    }
}
