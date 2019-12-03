using System;

namespace Extensions.EntityFramework.DataMigration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MigrationIdAttribute : Attribute
    {
        public MigrationIdAttribute(string migrationId)
        {
            MigrationId = migrationId;
        }

        public string MigrationId { get; }
    }
}