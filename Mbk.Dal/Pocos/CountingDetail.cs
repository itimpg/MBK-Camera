namespace Mbk.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CountingDetail")]
    public partial class CountingDetail
    {
        public long Id { get; set; }

        public long CountingId { get; set; }

        public long A { get; set; }

        public long B { get; set; }

        public virtual Counting Counting { get; set; }
    }
}
