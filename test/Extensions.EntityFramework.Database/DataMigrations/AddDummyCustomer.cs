using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extensions.EntityFramework.DataMigration;
using Extensions.Test.Model;

namespace Extensions.EntityFramework.Database.DataMigrations
{
    [MigrationId(MigrationId)]
    public class AddDummyCustomer : IDataMigration<TestContext>
    {
        public const string MigrationId = "D0000002_AddDummyCustomer";

        public async Task ApplyAsync(TestContext context, CancellationToken cancellationToken = default)
        {
            var countryId = await context.Countries.Where(p => p.Iso2Code == "US").Select(p => p.Id).FirstAsync();

            context.Customers.Add(new Customer() { FirstName = "Dummy", LastName = "Customer", CountryId = countryId, Id = Guid.NewGuid() });

            await context.SaveChangesAsync();
        }
    }
}