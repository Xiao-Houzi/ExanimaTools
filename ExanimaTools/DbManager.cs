// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using System;
using System.IO;
using ExanimaTools.Persistence;
using ExanimaTools.Models;
using Microsoft.Data.Sqlite;

namespace ExanimaTools
{
    public static class DbManager
    {
        private static string? _dbPath;
public static string DbFileName { get; } = "exanima_tools.db";
        public static string GetDbPath()
        {
            if (_dbPath == null)
            {
                // Use BaseDirectory for consistency
                _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbFileName);
            }
            return _dbPath;
        }

        public static void SetDbPath(string path)
        {
            _dbPath = path;
        }

        public static EquipmentRepository GetEquipmentRepository(ILoggingService logger, SqliteConnection? connection = null)
        {
            if (connection != null)
                return new EquipmentRepository(connection, logger);
            var dbPath = GetDbPath();
            return new EquipmentRepository($"Data Source={dbPath}", logger);
        }

        public static ArsenalRepository GetArsenalRepository(SqliteConnection? connection = null)
        {
            if (connection != null)
                return new ArsenalRepository(connection);
            var dbPath = GetDbPath();
            return new ArsenalRepository($"Data Source={dbPath}");
        }

        public static CompanyMemberRepository GetCompanyMemberRepository(ILoggingService logger, SqliteConnection? connection = null)
        {
            if (connection != null)
                return new CompanyMemberRepository(connection);
            var dbPath = GetDbPath();
            return new CompanyMemberRepository($"Data Source={dbPath}");
        }
    }
}
