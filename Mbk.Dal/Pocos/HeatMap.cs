namespace Mbk.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HeatMap")]
    public partial class HeatMap
    {
        public long Id { get; set; }

        public long CameraId { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Date { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Time { get; set; }

        public long Gmt { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string RawData { get; set; }

        public decimal Density { get; set; }
    }
}
