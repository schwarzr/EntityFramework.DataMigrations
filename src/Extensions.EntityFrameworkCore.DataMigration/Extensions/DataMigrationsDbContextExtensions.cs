using System;
using Extensions.EntityFrameworkCore.DataMigration;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public static class DataMigrationsDbContextExtensions
    {
        public static DataMigrator DataMigrator(this DbContext context)
        {
            return new DataMigrator(context, ((IInfrastructure<IServiceProvider>)context).GetService<DataMigrationOptions>());
        }
    }
}