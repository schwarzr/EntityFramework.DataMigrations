using System;
using Extensions.EntityFrameworkCore.DataMigration;
using Extensions.EntityFrameworkCore.DataMigration.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public static class DataMigrationsDbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder<TContext> UseDataMigrations<TContext>(this DbContextOptionsBuilder<TContext> options, Action<DataMigrationOptions> dataMigrationOptions = null)
            where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)((DbContextOptionsBuilder)options).UseDataMigrations(dataMigrationOptions);
        }

        public static DbContextOptionsBuilder UseDataMigrations(this DbContextOptionsBuilder options, Action<DataMigrationOptions> dataMigrationOptions = null)
        {
            var migrationOptions = new DataMigrationOptions();
            dataMigrationOptions?.Invoke(migrationOptions);
            var extensions = new DataMigrationOptionsExtension(migrationOptions);

            ((IDbContextOptionsBuilderInfrastructure)options).AddOrUpdateExtension(extensions);

            return options;
        }
    }
}