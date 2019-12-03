using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions.EntityFramework.DataMigration
{
    public interface IDataMigration<in TContext>
        where TContext : DbContext
    {
        Task ApplyAsync(TContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}