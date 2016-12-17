using Mbk.Enums;

namespace Mbk.Model
{
    public class ConfigModel
    {
        public string DatabaseSource { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

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
