using System.Data.Entity;
using Extensions.Test.Model;

namespace Extensions.EntityFramework.Database
{
    public class TestContext : BaseContext
    {
        public TestContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}