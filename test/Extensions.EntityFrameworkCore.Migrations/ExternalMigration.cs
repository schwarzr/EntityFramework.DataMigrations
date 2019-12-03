using System;
using System.Threading;
using System.Threading.Tasks;
using Extensions.EntityFrameworkCore.Database;
using Extensions.EntityFrameworkCore.DataMigration;

namespace Extensions.EntityFrameworkCore.Migrations
{
    public class ExternalMigration : IDataMigration<TestContext>
    {
        public async Task ApplyAsync(TestContext context, CancellationToken cancellationToken = default)
        {
            await context.Currencies.AddAsync(new Test.Model.Currency { Id = Guid.NewGuid(), IsoCode = "CHF", Title = "Swiss Franks" });

            await context.SaveChangesAsync();
        }
    }
}