using System;
using System.Data.Entity;

namespace Extensions.EntityFramework.DataMigraton.Test
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
                    _context.Database.Delete();
                }

                disposedValue = true;
            }
        }
    }
}