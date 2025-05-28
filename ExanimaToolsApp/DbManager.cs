using System;
using System.IO;

namespace ExanimaToolsApp
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
    }
}
