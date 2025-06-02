using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace ExanimaTools.Persistence
{
    public static class DbMigrationHelper
    {
        public static void MigrateEquipmentTable(string connectionString)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var columns = new HashSet<string>();
            using (var pragmaCmd = conn.CreateCommand())
            {
                pragmaCmd.CommandText = "PRAGMA table_info(Equipment)";
                using var reader = pragmaCmd.ExecuteReader();
                while (reader.Read())
                {
                    columns.Add(reader.GetString(1));
                }
            }
            // Add new columns here as needed for future migrations
            if (!columns.Contains("Rank"))
            {
                using var alter = conn.CreateCommand();
                alter.CommandText = "ALTER TABLE Equipment ADD COLUMN Rank INTEGER DEFAULT 0";
                alter.ExecuteNonQuery();
            }
            if (!columns.Contains("Points"))
            {
                using var alter = conn.CreateCommand();
                alter.CommandText = "ALTER TABLE Equipment ADD COLUMN Points INTEGER DEFAULT 0";
                alter.ExecuteNonQuery();
            }
            if (!columns.Contains("Weight"))
            {
                using var alter = conn.CreateCommand();
                alter.CommandText = "ALTER TABLE Equipment ADD COLUMN Weight REAL DEFAULT 0";
                alter.ExecuteNonQuery();
            }
            // Add more migration steps as needed
        }
    }
}
