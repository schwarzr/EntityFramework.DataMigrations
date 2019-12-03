using System;

namespace Extensions.EntityFrameworkCore.DataMigration
{
    public class DataMigrationOptions : IEquatable<DataMigrationOptions>
    {
        public string MigrationAssembly { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is DataMigrationOptions other)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public bool Equals(DataMigrationOptions other)
        {
            return object.Equals(other.MigrationAssembly, this.MigrationAssembly);
        }

        public override int GetHashCode()
        {
            return MigrationAssembly?.GetHashCode() ?? 0;
        }
    }
}