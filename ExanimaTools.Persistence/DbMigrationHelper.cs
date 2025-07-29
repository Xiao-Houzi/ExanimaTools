// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

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
            MigrateEquipmentTable(conn);
        }

        public static void MigrateEquipmentTable(SqliteConnection? externalConn)
        {
            var conn = externalConn ?? throw new ArgumentNullException(nameof(externalConn));
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
            
            // Check if Equipment table exists first
            bool tableExists;
            using (var checkCmd = conn.CreateCommand())
            {
                checkCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Equipment'";
                tableExists = checkCmd.ExecuteScalar() != null;
            }
            
            if (!tableExists)
            {
                // Table doesn't exist, no need to migrate
                return;
            }
            
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
            if (!columns.Contains("Stats"))
            {
                using var alter = conn.CreateCommand();
                alter.CommandText = "ALTER TABLE Equipment ADD COLUMN Stats TEXT DEFAULT '{}'";
                alter.ExecuteNonQuery();
            }
        }
        
        public static void MigrateMemberEquipmentTable(SqliteConnection? externalConn)
        {
            var conn = externalConn ?? throw new ArgumentNullException(nameof(externalConn));
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
            
            // Check if MemberEquipment table exists first
            bool tableExists;
            using (var checkCmd = conn.CreateCommand())
            {
                checkCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='MemberEquipment'";
                tableExists = checkCmd.ExecuteScalar() != null;
            }
            
            if (!tableExists)
            {
                // Table doesn't exist, no need to migrate
                return;
            }
            
            var columns = new HashSet<string>();
            using (var pragmaCmd = conn.CreateCommand())
            {
                pragmaCmd.CommandText = "PRAGMA table_info(MemberEquipment)";
                using var reader = pragmaCmd.ExecuteReader();
                while (reader.Read())
                {
                    columns.Add(reader.GetString(1));
                }
            }
            
            // Add Rank column if it doesn't exist
            if (!columns.Contains("Rank"))
            {
                using var alter = conn.CreateCommand();
                alter.CommandText = "ALTER TABLE MemberEquipment ADD COLUMN Rank INTEGER DEFAULT 0";
                alter.ExecuteNonQuery();
            }
        }
    }
}
