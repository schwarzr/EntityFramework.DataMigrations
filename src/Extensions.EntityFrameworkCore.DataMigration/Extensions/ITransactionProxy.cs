using System;

namespace Extensions.EntityFrameworkCore.DataMigration.Extensions
{
    internal interface ITransactionProxy : IDisposable
    {
        void Commit();
    }
}