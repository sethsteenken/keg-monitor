using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace KegMonitor.Infrastructure.EntityFramework
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder PluralizeTableNames(this ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes().Where(e => !e.ClrType.IsAbstract);
            foreach (var entityType in entityTypes)
            {
                var tableName = entityType.GetTableName();
                entityType.SetTableName(tableName.Pluralize(inputIsKnownToBeSingular: false));
            }

            return modelBuilder;
        }

        public static ModelBuilder SetSnakeCase(this ModelBuilder modelBuilder)
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
                        StoreObjectIdentifier.Table(entityType.GetTableName() ?? string.Empty, property.DeclaringType.GetSchema()));
                    property.SetColumnName(columnName.Underscore());
                }
            }

            return modelBuilder;
        }
    }
}
