using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extensions.EntityFrameworkCore.DataMigration;
using Extensions.Test.Model;
using Microsoft.EntityFrameworkCore;

namespace Extensions.EntityFrameworkCore.Database
{
    [MigrationId(MigrationId)]
    public class AddDummyCustomer : IDataMigration<TestContext>
    {
        public const string MigrationId = "D0000002_AddDummyCustomer";

        public async Task ApplyAsync(TestContext context, CancellationToken cancellationToken = default)
        {
            var countryId = await context.Countries.Where(p => p.Iso2Code == "US").Select(p => p.Id).FirstAsync();

            await context.Customers.AddAsync(new Customer() { FirstName = "Dummy", LastName = "Customer", CountryId = countryId, Id = Guid.NewGuid() });

            await context.SaveChangesAsync();
        }
    }
}