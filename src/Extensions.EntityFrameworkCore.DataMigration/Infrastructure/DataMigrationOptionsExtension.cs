using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.EntityFrameworkCore.DataMigration.Infrastructure
{
    public class DataMigrationOptionsExtension : IDbContextOptionsExtension
    {
        private readonly DataMigrationOptions _migrationOptions;

        public DataMigrationOptionsExtension(DataMigrationOptions migrationOptions)
        {
            _migrationOptions = migrationOptions;

            Info = new DataMigrationOptionsExtensionInfo(this);
        }

        public DbContextOptionsExtensionInfo Info { get; }

        public void ApplyServices(IServiceCollection services)
        {
            services.AddSingleton(_migrationOptions);
            var oldCustomizer = services.First(p => p.ServiceType == typeof(IModelCustomizer));
            services.Remove(oldCustomizer);
            services.Add(ServiceDescriptor.Describe(oldCustomizer.ImplementationType, oldCustomizer.ImplementationType, oldCustomizer.Lifetime));
            var newCustomizer = typeof(DataMigrationModelCustomizer<>).MakeGenericType(oldCustomizer.ImplementationType);
            services.Add(ServiceDescriptor.Describe(typeof(IModelCustomizer), newCustomizer, oldCustomizer.Lifetime));
        }

        public virtual void Validate(IDbContextOptions options)
        {
        }

        private class DataMigrationOptionsExtensionInfo : DbContextOptionsExtensionInfo
        {
            private readonly DataMigrationOptionsExtension _extension;

            public DataMigrationOptionsExtensionInfo(DataMigrationOptionsExtension extension)
                : base(extension)
            {
                _extension = extension;
            }

            public override bool IsDatabaseProvider => false;

            public override string LogFragment => "SqlServerBulk";

#if EF6_0
            public override int GetServiceProviderHashCode()
#else
            public override long GetServiceProviderHashCode()
#endif
            {
                return _extension._migrationOptions.GetHashCode() * 9909;
            }

            public override void PopulateDebugInfo(System.Collections.Generic.IDictionary<string, string> debugInfo)
            {
                if (!string.IsNullOrWhiteSpace(_extension._migrationOptions.MigrationAssembly))
                {
                    debugInfo.Add("DataMigrations.MigrationAssembly", _extension._migrationOptions.MigrationAssembly);
                }
            }

#if EF6_0
            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                return true;
            }
#endif
        }
    }
}