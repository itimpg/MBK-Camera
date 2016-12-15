using System;

namespace Mbk.Model
{
    public class CountingModel
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Gmt { get; set; }
        public string RawData { get; set; }
        public decimal Population { get; set; }
    }
}
