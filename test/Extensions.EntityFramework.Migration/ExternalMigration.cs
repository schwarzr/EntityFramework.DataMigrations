using System;
using System.Threading;
using System.Threading.Tasks;
using Extensions.EntityFramework.Database;
using Extensions.EntityFramework.DataMigration;

namespace Extensions.EntityFrameworkCore.Migrations
{
    public class ExternalMigration : IDataMigration<TestContext>
    {
        public async Task ApplyAsync(TestContext context, CancellationToken cancellationToken = default)
        {
            context.Currencies.Add(new Test.Model.Currency { Id = Guid.NewGuid(), IsoCode = "CHF", Title = "Swiss Franks" });

            await context.SaveChangesAsync();
        }
    }
}