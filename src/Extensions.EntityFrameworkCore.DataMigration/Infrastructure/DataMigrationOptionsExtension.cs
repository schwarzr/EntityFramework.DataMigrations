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
#if !EF2_2
            Info = new DataMigrationOptionsExtensionInfo(this);
#endif
        }

#if EF2_2
        public string LogFragment => "SqlServerBulk";
#else
        public DbContextOptionsExtensionInfo Info { get; }
#endif

#if EF2_2
        public bool ApplyServices(IServiceCollection services)
#else
        public void ApplyServices(IServiceCollection services)
#endif
        {
            services.AddSingleton(_migrationOptions);
            var oldCustomizer = services.First(p => p.ServiceType == typeof(IModelCustomizer));
            services.Remove(oldCustomizer);
            services.Add(ServiceDescriptor.Describe(oldCustomizer.ImplementationType, oldCustomizer.ImplementationType, oldCustomizer.Lifetime));
            var newCustomizer = typeof(DataMigrationModelCustomizer<>).MakeGenericType(oldCustomizer.ImplementationType);
            services.Add(ServiceDescriptor.Describe(typeof(IModelCustomizer), newCustomizer, oldCustomizer.Lifetime));

#if EF2_2
            return true;
#endif
        }

        public virtual void Validate(IDbContextOptions options)
        {
        }

#if EF2_2
        public virtual long GetServiceProviderHashCode()
        {
            return _migrationOptions.GetHashCode() * 9909;
        }
#else
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

            public override long GetServiceProviderHashCode()
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
        }
#endif
    }
}