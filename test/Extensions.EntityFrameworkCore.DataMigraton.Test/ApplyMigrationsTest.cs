using System;
using System.Linq;
using System.Threading.Tasks;
using Extensions.EntityFrameworkCore.Database;
using Extensions.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Extensions.EntityFrameworkCore.DataMigraton.Test
{
    public class ApplyMigrationsTest
    {
        public ApplyMigrationsTest()
        {
            DatabaseId = Guid.NewGuid();
        }

        public Guid DatabaseId { get; }

        [Theory]
        [InlineData(new object[] { (object)DatabaseTarget.ImMemory })]
        [InlineData(new object[] { (object)DatabaseTarget.SqlServer })]
        public async Task ApplyMigratonsFromDefaultAndExternalAssembly_ExpectsOk(DatabaseTarget target)
        {
            using (var sp = CreateServiceProvider(target))
            {
                var ctx = sp.GetRequiredService<TestContext>();
                await ctx.Database.EnsureCreatedAsync();

                var migrator = ctx.DataMigrator();
                await migrator.MigrateAsync();

                using (var spExternal = CreateServiceProvider(target, true, false))
                {
                    var ctxExternal = spExternal.GetRequiredService<TestContext>();
                    var migratorExternal = ctxExternal.DataMigrator();
                    await migratorExternal.MigrateAsync();

                    var applied = await migrator.GetAppliedMigrationsAsync();
                    applied = applied.Concat(await migratorExternal.GetAppliedMigrationsAsync());

                    Assert.Equal(new[] { nameof(D0000001_InitialDataMigration), AddDummyCustomer.MigrationId, nameof(ExternalMigration) }, applied);
                }
            }
        }

        [Theory]
        [InlineData(new object[] { (object)DatabaseTarget.ImMemory })]
        [InlineData(new object[] { (object)DatabaseTarget.SqlServer })]
        public async Task ApplyMigratonsFromDefaultAssembly_ExpectsOk(DatabaseTarget target)
        {
            using (var sp = CreateServiceProvider(target))
            {
                var ctx = sp.GetRequiredService<TestContext>();

                await ctx.Database.EnsureCreatedAsync();

                var migrator = ctx.DataMigrator();
                await migrator.MigrateAsync();

                var applied = await migrator.GetAppliedMigrationsAsync();

                Assert.Equal(new[] { nameof(D0000001_InitialDataMigration), AddDummyCustomer.MigrationId }, applied);
            }
        }

        [Theory]
        [InlineData(new object[] { (object)DatabaseTarget.ImMemory })]
        [InlineData(new object[] { (object)DatabaseTarget.SqlServer })]
        public async Task ApplyMigratonsFromExternalAssembly_ExpectsOk(DatabaseTarget target)
        {
            using (var sp = CreateServiceProvider(target, true))
            {
                var ctx = sp.GetRequiredService<TestContext>();

                await ctx.Database.EnsureCreatedAsync();

                var migrator = ctx.DataMigrator();
                await migrator.MigrateAsync();

                var applied = await migrator.GetAppliedMigrationsAsync();
                var data = await ctx.Currencies.ToListAsync();

                Assert.Equal(new[] { nameof(ExternalMigration) }, applied);
                Assert.Single(data);
                Assert.Equal("CHF", data[0].IsoCode);
            }
        }

        [Theory]
        [InlineData(new object[] { (object)DatabaseTarget.ImMemory })]
        [InlineData(new object[] { (object)DatabaseTarget.SqlServer })]
        public async Task ApplyMigratonsWithSurroundingTransaction_ExpectsOk(DatabaseTarget target)
        {
            using (var sp = CreateServiceProvider(target))
            {
                var ctx = sp.GetRequiredService<TestContext>();
                await ctx.Database.EnsureCreatedAsync();

                var migrator = ctx.DataMigrator();

                using (var transaction = await ctx.Database.BeginTransactionAsync())
                {
                    await migrator.MigrateAsync();
                    transaction.Commit();
                }

                var applied = await migrator.GetAppliedMigrationsAsync();

                Assert.Equal(new[] { nameof(D0000001_InitialDataMigration), AddDummyCustomer.MigrationId }, applied);
            }
        }

        [Theory]
        [InlineData(new object[] { (object)DatabaseTarget.SqlServer })]
        public async Task ApplyMigratonsWithSurroundingTransactionRollback_ExpectsEmptyDb(DatabaseTarget target)
        {
            using (var sp = CreateServiceProvider(target))
            {
                var ctx = sp.GetRequiredService<TestContext>();
                await ctx.Database.EnsureCreatedAsync();

                var migrator = ctx.DataMigrator();

                using (var transaction = await ctx.Database.BeginTransactionAsync())
                {
                    await migrator.MigrateAsync();
                }

                var applied = await migrator.GetAppliedMigrationsAsync();

                Assert.Empty(applied);
            }
        }

        private ServiceProvider CreateServiceProvider(DatabaseTarget target, bool useExternalMigrations = false, bool addDatabaseDeleter = true)
        {
            var services = new ServiceCollection();

            if (target == DatabaseTarget.ImMemory)
            {
                services.AddDbContext<TestContext>(builder =>
                    builder.UseInMemoryDatabase("UnitTest")
                        .ConfigureWarnings(options => options.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                        .UseDataMigrations(p => p.MigrationAssembly = useExternalMigrations ? "Extensions.EntityFrameworkCore.Migrations" : null)
                    );
            }
            else
            {
                services.AddDbContext<TestContext>(builder =>
                builder.UseSqlServer($"Data Source=(localdb)\\mssqllocaldb; Integrated Security=true; Initial Catalog={DatabaseId:N}")
                    .UseDataMigrations(p => p.MigrationAssembly = useExternalMigrations ? "Extensions.EntityFrameworkCore.Migrations" : null)
                );
            }

            if (addDatabaseDeleter)
            {
                services.AddSingleton<DropDatabase<TestContext>>();
            }

            var result = services.BuildServiceProvider();

            if (addDatabaseDeleter)
            {
                result.GetRequiredService<DropDatabase<TestContext>>();
            }

            return result;
        }

        // To detect redundant calls
    }
}