using System.Threading;
using System.Threading.Tasks;
using Extensions.EntityFrameworkCore.DataMigration.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore
{
    internal static class ExtensionsEntityFrameworkDataMigrationsDatabaseExtensions
    {
        internal static async Task<ITransactionProxy> BeginTransactionIfNotYetRunnigAsync(this DatabaseFacade database, CancellationToken cancellationToken = default)
        {
            if (database.CurrentTransaction == null)
            {
                return new TransactionProxy(await database.BeginTransactionAsync(cancellationToken));
            }

            return new TransactionProxy();
        }

        private class TransactionProxy : ITransactionProxy
        {
            private readonly IDbContextTransaction _underlying;
            private bool _disposedValue = false;

            public TransactionProxy()
            {
            }

            public TransactionProxy(IDbContextTransaction underlying)
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