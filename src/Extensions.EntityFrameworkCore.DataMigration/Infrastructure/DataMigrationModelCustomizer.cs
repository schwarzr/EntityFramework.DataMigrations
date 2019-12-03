using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Extensions.EntityFrameworkCore.DataMigration.Infrastructure
{
    public class DataMigrationModelCustomizer<TBaseCustomizer> : IModelCustomizer
        where TBaseCustomizer : IModelCustomizer
    {
        private readonly TBaseCustomizer _baseCustomizer;

        public DataMigrationModelCustomizer(TBaseCustomizer baseCustomizer)
        {
            _baseCustomizer = baseCustomizer;
        }

        public void Customize(ModelBuilder modelBuilder, DbContext context)
        {
            modelBuilder.Entity<DataMigrationHistoryRow>()
                .Metadata.AddAnnotation("Relational:TableName", "__DataMigrationHistory");
            _baseCustomizer.Customize(modelBuilder, context);
        }
    }
}