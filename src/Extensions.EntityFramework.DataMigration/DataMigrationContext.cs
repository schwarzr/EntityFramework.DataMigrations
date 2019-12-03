using System.Data.Common;
using System.Data.Entity;

namespace Extensions.EntityFramework.DataMigration
{
    public class DataMigrationContext : DbContext
    {
        static DataMigrationContext()
        {
            Database.SetInitializer<DataMigrationContext>(new CreateDatabaseIfNotExists<DataMigrationContext>());
        }

        public DataMigrationContext(DbConnection existingConnection)
            : base(existingConnection, false)
        {
        }

        public DbSet<DataMigrationHistoryRow> HistoryRows { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataMigrationHistoryRow>().ToTable("__DataMigrationHistory");

            base.OnModelCreating(modelBuilder);
        }
    }
}