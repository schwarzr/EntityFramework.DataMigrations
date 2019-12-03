using System;

namespace Extensions.EntityFramework.DataMigration.Extensions
{
    internal interface ITransactionProxy : IDisposable
    {
        void Commit();
    }
}