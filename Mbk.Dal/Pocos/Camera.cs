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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Camera()
        {
            Countings = new HashSet<Counting>();
            HeatMaps = new HashSet<HeatMap>();
        }

        public long Id { get; set; }

        [StringLength(2147483647)]
        public string Floor { get; set; }

        [StringLength(2147483647)]
        public string Name { get; set; }

        public decimal Height { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string IpAddress { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Counting> Countings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeatMap> HeatMaps { get; set; }
    }
}
