namespace Mbk.Dal
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.SQLite;

    public partial class MbkCameraDb : DbContext
    {
        public MbkCameraDb(string connectionString)
            : base(new SQLiteConnection() { ConnectionString = "data source=" + connectionString }, true)
        {

        }

        public virtual DbSet<Camera> Cameras { get; set; }
        public virtual DbSet<Counting> Countings { get; set; }
        public virtual DbSet<CountingDetail> CountingDetails { get; set; }
        public virtual DbSet<HeatMap> HeatMaps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Camera>()
                .Property(e => e.Height)
                .HasPrecision(53, 0);

            modelBuilder.Entity<Counting>()
                .HasMany(e => e.CountingDetails)
                .WithRequired(e => e.Counting)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HeatMap>()
                .Property(e => e.Density)
                .HasPrecision(53, 0);
        }
    }
}
