using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Extensions.EntityFramework.Database;
using Extensions.EntityFramework.Database.DataMigrations;
using Extensions.EntityFramework.DataMigration;
using Extensions.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Extensions.EntityFramework.DataMigraton.Test
{
    public class ApplyMigrationsTest
    {
        public ApplyMigrationsTest()
        {
        }

        [Fact]
        public async Task ApplyMigratonsFromDefaultAndExternalAssembly_ExpectsOk()
        {
            using (var sp = CreateServiceProvider())
            {
                var ctx = sp.GetRequiredService<TestContext>();
                ctx.Database.CreateIfNotExists();

                var migrator = new DataMigrator(ctx, new DataMigrationOptions());
                await migrator.MigrateAsync();

                var migratorExternal = new DataMigrator(ctx, new DataMigrationOptions { MigrationAssembly = "Extensions.EntityFramework.Migration" });
                await migratorExternal.MigrateAsync();

                var applied = await migrator.GetAppliedMigrationsAsync();
                applied = applied.Concat(await migratorExternal.GetAppliedMigrationsAsync());

                Assert.Equal(new[] { nameof(D0000001_InitialDataMigration), AddDummyCustomer.MigrationId, nameof(ExternalMigration) }, applied);
            }
        }

        [Fact]
        public async Task ApplyMigratonsFromDefaultAssembly_ExpectsOk()
        {
            using (var sp = CreateServiceProvider())
            {
                var ctx = sp.GetRequiredService<TestContext>();
                ctx.Database.CreateIfNotExists();

                var migrator = new DataMigrator(ctx, new DataMigrationOptions());
                await migrator.MigrateAsync();

                var applied = await migrator.GetAppliedMigrationsAsync();

                Assert.Equal(new[] { nameof(D0000001_InitialDataMigration), AddDummyCustomer.MigrationId }, applied);
            }
        }

        [Fact]
        public async Task ApplyMigratonsFromExternalAssembly_ExpectsOk()
        {
            using (var sp = CreateServiceProvider())
            {
                var ctx = sp.GetRequiredService<TestContext>();

                ctx.Database.CreateIfNotExists();

                var migrator = new DataMigrator(ctx, new DataMigrationOptions { MigrationAssembly = "Extensions.EntityFramework.Migration" });
                await migrator.MigrateAsync();

                var applied = await migrator.GetAppliedMigrationsAsync();
                var data = await ctx.Currencies.ToListAsync();

                Assert.Equal(new[] { nameof(ExternalMigration) }, applied);
                Assert.Single(data);
                Assert.Equal("CHF", data[0].IsoCode);
            }
        }

        [Fact]
        public async Task ApplyMigratonsWithSurroundingTransaction_ExpectsOk()
        {
            using (var sp = CreateServiceProvider())
            {
                var ctx = sp.GetRequiredService<TestContext>();
                ctx.Database.Create();

                var migrator = new DataMigrator(ctx, new DataMigrationOptions());

                using (var transaction = ctx.Database.BeginTransaction())
                {
                    await migrator.MigrateAsync();
                    transaction.Commit();
                }

                var applied = await migrator.GetAppliedMigrationsAsync();

                Assert.Equal(new[] { nameof(D0000001_InitialDataMigration), AddDummyCustomer.MigrationId }, applied);
            }
        }

        [Fact]
        public async Task ApplyMigratonsWithSurroundingTransactionRollback_ExpectsEmptyDb()
        {
            using (var sp = CreateServiceProvider())
            {
                var ctx = sp.GetRequiredService<TestContext>();
                ctx.Database.Create();

                var migrator = new DataMigrator(ctx, new DataMigrationOptions());

                using (var transaction = ctx.Database.BeginTransaction())
                {
                    await migrator.MigrateAsync();
                }

                var applied = await migrator.GetAppliedMigrationsAsync();

                Assert.Empty(applied);
            }
        }

        private static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton<TestContext>(sp => new TestContext($"Data Source=(localdb)\\mssqllocaldb; Integrated Security=true; Initial Catalog={Guid.NewGuid():N}"));
            services.AddSingleton<DropDatabase<TestContext>>();

            var result = services.BuildServiceProvider();

            result.GetRequiredService<DropDatabase<TestContext>>();

            return result;
        }

        // To detect redundant calls
    }
}