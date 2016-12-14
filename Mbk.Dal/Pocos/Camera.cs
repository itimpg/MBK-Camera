namespace Mbk.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Camera")]
    public partial class Camera
    {
        public long Id { get; set; }

        [StringLength(2147483647)]
        public string Floor { get; set; }

        [StringLength(2147483647)]
        public string Name { get; set; }

        public decimal Height { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string IpAddress { get; set; }
    }
}
