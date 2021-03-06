﻿using System.ComponentModel.DataAnnotations;

namespace Extensions.EntityFramework.DataMigration
{
    public class DataMigrationHistoryRow
    {
        [StringLength(150)]
        [Key]
        public string MigrationId { get; set; }
    }
}