using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Extensions.EntityFrameworkCore.DataMigration
{
    public interface IDataMigration<in TContext>
        where TContext : DbContext
    {
        Task ApplyAsync(TContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}