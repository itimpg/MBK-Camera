using Mbk.Enums;

namespace Mbk.Model
{
    public class ConfigModel
    {
        public string ServiceName { get; set; }
        public string DatabaseSource { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string HeatMapUri { get; set; }
        public string HeatMapBufferFileName { get; set; }
        public string CountingUri { get; set; }
        public string CountingBufferFileName { get; set; }

        public ScheduleConfigModel ExportConfig { get; set; }
        public ScheduleConfigModel DataConfig { get; set; }
    }

    public class ScheduleConfigModel
    {
        public string Location { get; set; }
        public bool IsEnabled { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public ReportPeriodType Period { get; set; }
    }
}
