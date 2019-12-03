using Extensions.EntityFramework.DataMigration.Extensions;

namespace System.Data.Entity
{
    internal static class ExtensionsEntityFrameworkDataMigrationsDatabaseExtensions
    {
        internal static ITransactionProxy BeginTransactionIfNotYetRunnig(this Database database)
        {
            if (database.CurrentTransaction == null)
            {
                return new TransactionProxy(database.BeginTransaction());
            }

            return new TransactionProxy();
        }

        private class TransactionProxy : ITransactionProxy
        {
            private readonly DbContextTransaction _underlying;
            private bool _disposedValue = false;

            public TransactionProxy()
            {
            }

            public TransactionProxy(DbContextTransaction underlying)
            {
                this._underlying = underlying;
            }

            public void Commit()
            {
                this._underlying?.Commit();
            }

            public void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposedValue)
                {
                    if (disposing)
                    {
                        _underlying?.Dispose();
                    }

                    _disposedValue = true;
                }
            }
        }
    }
}