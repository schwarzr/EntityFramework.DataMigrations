using System.Data.Entity;
using Extensions.Test.Model;

namespace Extensions.EntityFramework.Database
{
    public class BaseContext : DbContext
    {
        public BaseContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DbSet<City> Cities { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Currency> Currencies { get; set; }
    }
}