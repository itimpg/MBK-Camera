namespace Mbk.Dal
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MbkCameraDb : DbContext
    {
        public MbkCameraDb()
            : base("name=MbkCameraDb")
        {
        }

        public virtual DbSet<Camera> Cameras { get; set; }
        public virtual DbSet<Counting> Countings { get; set; }
        public virtual DbSet<HeatMap> HeatMaps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Camera>()
                .Property(e => e.Height)
                .HasPrecision(53, 0);

            modelBuilder.Entity<Camera>()
                .HasMany(e => e.Countings)
                .WithRequired(e => e.Camera)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Camera>()
                .HasMany(e => e.HeatMaps)
                .WithRequired(e => e.Camera)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Counting>()
                .Property(e => e.Population)
                .HasPrecision(53, 0);

            modelBuilder.Entity<HeatMap>()
                .Property(e => e.Density)
                .HasPrecision(53, 0);
        }
    }
}
