using KegMonitor.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Infrastructure.EntityFramework
{
    public class KegMonitorDbContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public KegMonitorDbContext(DbContextOptions options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.PluralizeTableNames();
            modelBuilder.SetSnakeCase();

            modelBuilder.Entity<Beer>().Property(b => b.Name).HasMaxLength(200);
            modelBuilder.Entity<Beer>().Property(b => b.Type).HasMaxLength(200);

            modelBuilder.Entity<Scale>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<Scale>().HasOne(s => s.Beer).WithMany();
            modelBuilder.Entity<ScaleWeightChange>().HasOne(swc => swc.Scale).WithMany(s => s.WeightChanges);
            modelBuilder.Entity<ScaleWeightChange>().HasOne(swc => swc.Beer).WithMany().OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Scale> Scales { get; set; }
        public DbSet<Beer> Beers { get; set; }
        public DbSet<ScaleWeightChange> ScaleWeightChanges { get; set; }
    }
}