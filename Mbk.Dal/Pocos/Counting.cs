namespace Mbk.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Counting")]
    public partial class Counting
    {
        public long Id { get; set; }

        public long CameraId { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Date { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Time { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Gmt { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string RawData { get; set; }

        public decimal Population { get; set; }
    }
}
