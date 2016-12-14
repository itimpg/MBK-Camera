using System;

namespace Mbk.Model
{
    public class HeatMapModel
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public DateTime DateTime { get; set; }
        public int Gmt { get; set; }
        public string RawData { get; set; }
        public decimal Density { get; set; }
    }
}
