using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Extensions.EntityFrameworkCore.DataMigraton.Test
{
    public class DropDatabase<TContext> : IDisposable
        where TContext : DbContext
    {
        private readonly TContext _context;

        private bool disposedValue = false;

        public DropDatabase(TContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Database.EnsureDeleted();
                }

                disposedValue = true;
            }
        }
    }
}