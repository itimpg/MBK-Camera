using System;

namespace Mbk.Model
{
    public class HeatMapModel
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Gmt { get; set; }
        public string RawData { get; set; }
        public decimal Density { get; set; }
        public long TotalValue { get; set; }
        public long TotalCount { get; set; }
    }
}
