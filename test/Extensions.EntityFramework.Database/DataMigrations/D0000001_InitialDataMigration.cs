using System;
using System.Threading;
using System.Threading.Tasks;
using Extensions.EntityFramework.DataMigration;
using Extensions.Test.Model;

namespace Extensions.EntityFramework.Database.DataMigrations
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
                new Country() { Name = "Austria", Iso2Code = "AT", Iso3Code = "AUT", CurrencyId = currencies[0].Id },
                new Country() { Name = "Spain", Iso2Code = "ES", Iso3Code = "ESP", CurrencyId = currencies[0].Id },
                new Country() { Name = "Great Britain", Iso2Code = "GB", Iso3Code = "GBR", CurrencyId = currencies[1].Id },
                new Country() { Name = "United States", Iso2Code = "US", Iso3Code = "USA", CurrencyId = currencies[2].Id },
            };

            context.Currencies.AddRange(currencies);
            await context.SaveChangesAsync();

            context.Countries.AddRange(countries);
            await context.SaveChangesAsync();
        }
    }
}