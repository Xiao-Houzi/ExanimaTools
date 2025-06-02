using System;
using System.IO;

namespace ExanimaToolsApp
{
    public static class DbManager
    {
        private static string? _dbPath;
#if DEBUG
        public static string DbFileName { get; } = "exanima_tools_dev.db";
#else
        public static string DbFileName { get; } = "exanima_tools.db";
#endif
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
