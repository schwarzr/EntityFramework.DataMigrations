using System;
using Extensions.EntityFrameworkCore.Database;
using Extensions.EntityFrameworkCore.DataMigration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Extensions.EntityFrameworkCore.DataMigraton.Test
{
    public class DataMigratorModelCustomizerTest : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        private bool disposedValue = false;

        public DataMigratorModelCustomizerTest()
        {
            _serviceProvider = CreateServiceProvider();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        [Fact]
        public void ModelCustomizerAddsDataMigrationHistoryRow_ExpectsOk()
        {
            var ctx = _serviceProvider.GetRequiredService<TestContext>();

            var entity = ctx.Model.FindEntityType(typeof(DataMigrationHistoryRow));

            Assert.NotNull(entity);
#if EF2_2
            Assert.Equal("__DataMigrationHistory", entity.Relational().TableName);
#else
            Assert.Equal("__DataMigrationHistory", entity.GetTableName());
#endif
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_serviceProvider is IDisposable disp)
                    {
                        disp.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        private static IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddDbContext<TestContext>(builder =>
                builder.UseInMemoryDatabase("UnitTest")
                    .UseDataMigrations()
                );

            return services.BuildServiceProvider();
        }

        // To detect redundant calls
    }
}