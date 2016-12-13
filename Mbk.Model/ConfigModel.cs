using Mbk.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbk.Model
{
    public class ConfigModel
    {
        public ScheduleConfig ExportConfig { get; set; }
        public ScheduleConfig DataConfig { get; set; }
    }

    public class ScheduleConfig
    {
        public string Location { get; set; }
        public bool IsEnabled { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public ReportPeriodType Period { get; set; }
    }
}
