using System;
using System.Threading;
using System.Threading.Tasks;
using Extensions.EntityFrameworkCore.DataMigration;
using Extensions.Test.Model;
using Microsoft.EntityFrameworkCore;

namespace Extensions.EntityFrameworkCore.Database
{
    public class D0000001_InitialDataMigration : IDataMigration<BaseContext>
    {
        public async Task ApplyAsync(BaseContext context, CancellationToken cancellationToken = default)
        {
            var currencies = new[]
            {
                new Currency { Id = Guid.NewGuid(), IsoCode = "EUR", Title = "Euro" },
                new Currency { Id = Guid.NewGuid(), IsoCode = "GBP", Title = "Pound" },
                new Currency { Id = Guid.NewGuid(), IsoCode = "USD", Title = "Dollar" },
            };

            var countries = new[]
            {
                new Country() { Id = 1, Name = "Austria", Iso2Code = "AT", Iso3Code = "AUT", CurrencyId = currencies[0].Id },
                new Country() { Id = 2, Name = "Spain", Iso2Code = "ES", Iso3Code = "ESP", CurrencyId = currencies[0].Id },
                new Country() { Id = 3, Name = "Great Britain", Iso2Code = "GB", Iso3Code = "GBR", CurrencyId = currencies[1].Id },
                new Country() { Id = 4, Name = "United States", Iso2Code = "US", Iso3Code = "USA", CurrencyId = currencies[2].Id },
            };

            await context.Currencies.AddRangeAsync(currencies);
            await context.SaveChangesAsync();

            if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                var statement = $"SET IDENTITY_INSERT {context.Model.FindEntityType(typeof(Country)).Relational().TableName} ON";
#pragma warning disable EF1000 // Possible SQL injection vulnerability.
                await context.Database.ExecuteSqlCommandAsync(statement);
#pragma warning restore EF1000 // Possible SQL injection vulnerability.
            }

            await context.Countries.AddRangeAsync(countries);
            await context.SaveChangesAsync();

            if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                var statement = $"SET IDENTITY_INSERT {context.Model.FindEntityType(typeof(Country)).Relational().TableName} OFF";
#pragma warning disable EF1000 // Possible SQL injection vulnerability.
                await context.Database.ExecuteSqlCommandAsync(statement);
#pragma warning restore EF1000 // Possible SQL injection vulnerability.
            }
        }
    }
}