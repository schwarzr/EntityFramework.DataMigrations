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
        }

        public string LogFragment => "SqlServerBulk";

        public bool ApplyServices(IServiceCollection services)
        {
            services.AddSingleton(_migrationOptions);
            var oldCustomizer = services.First(p => p.ServiceType == typeof(IModelCustomizer));
            services.Remove(oldCustomizer);
            services.Add(ServiceDescriptor.Describe(oldCustomizer.ImplementationType, oldCustomizer.ImplementationType, oldCustomizer.Lifetime));
            var newCustomizer = typeof(DataMigrationModelCustomizer<>).MakeGenericType(oldCustomizer.ImplementationType);
            services.Add(ServiceDescriptor.Describe(typeof(IModelCustomizer), newCustomizer, oldCustomizer.Lifetime));

            return true;
        }

        public virtual long GetServiceProviderHashCode()
        {
            return _migrationOptions.GetHashCode() * 9909;
        }

        public virtual void Validate(IDbContextOptions options)
        {
        }
    }
}