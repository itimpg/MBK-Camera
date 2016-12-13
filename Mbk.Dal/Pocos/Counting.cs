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

        [StringLength(2147483647)]
        public string Time { get; set; }

        public long? Gmt { get; set; }

        public decimal Population { get; set; }

        public virtual Camera Camera { get; set; }
    }
}