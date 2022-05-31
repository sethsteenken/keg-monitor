using Humanizer;
using KegMonitor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KegMonitor.Infrastructure.EntityFramework
{
    public class KegMonitorDbContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public KegMonitorDbContext()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=192.168.1.11;Database=keg-monitor;Username=keg-monitor-user;Password=beer1!");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            PluralizeTableNames(modelBuilder);
            SetSnakeCase(modelBuilder);

            modelBuilder.Entity<Beer>().Property(b => b.Name).HasMaxLength(200);
            modelBuilder.Entity<Beer>().Property(b => b.Type).HasMaxLength(200);

            modelBuilder.Entity<Scale>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<Scale>().HasOne(s => s.Beer).WithMany();
            modelBuilder.Entity<ScaleWeightChange>().HasOne(swc => swc.Scale).WithMany(s => s.WeightChanges);
        }

        public DbSet<Scale> Scales { get; set; }
        public DbSet<Beer> Beers { get; set; }
        public DbSet<ScaleWeightChange> ScaleWeightChanges { get; set; }

        private static ModelBuilder PluralizeTableNames(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes().Where(e => !e.ClrType.IsAbstract);
            foreach (var entityType in entityTypes)
            {
                var tableName = entityType.GetTableName();
                entityType.SetTableName(tableName.Pluralize(inputIsKnownToBeSingular: false));
            }

            return modelBuilder;
        }

        private static ModelBuilder SetSnakeCase(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes().Where(e => !e.ClrType.IsAbstract);

            foreach (var entityType in entityTypes)
            {
                var tableName = entityType.GetTableName();
                entityType.SetTableName(tableName.Underscore());
            }

            var alltypes = modelBuilder.Model.GetEntityTypes();

            foreach (var entityType in alltypes)
            {
                var properties = entityType.GetProperties();
                foreach (var property in properties)
                {
                    var columnName = property.GetColumnName(
                        StoreObjectIdentifier.Table(entityType.GetTableName() ?? string.Empty, property.DeclaringEntityType.GetSchema()));
                    property.SetColumnName(columnName.Underscore());
                }
            }

            return modelBuilder;
        }
    }
}