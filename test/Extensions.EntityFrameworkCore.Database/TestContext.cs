using Extensions.Test.Model;
using Microsoft.EntityFrameworkCore;

namespace Extensions.EntityFrameworkCore.Database
{
    public class TestContext : BaseContext
    {
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}