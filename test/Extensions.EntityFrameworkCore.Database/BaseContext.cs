using Extensions.Test.Model;
using Microsoft.EntityFrameworkCore;

namespace Extensions.EntityFrameworkCore.Database
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Currency> Currencies { get; set; }
    }
}